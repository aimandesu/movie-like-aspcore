using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using application.IRepository;
using domain.Entities;
using infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository
{
    public partial class SeriesRepository : ISeriesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        public SeriesRepository(
            ApplicationDbContext context,
            IFileService fileService
        )
        {
            _context = context;
            _fileService = fileService;

        }

        public async Task<Series?> CreateSeries(Series series, Stream thumbnail, string fileName)
        {
            var currentSeries = await _context.Series.ToListAsync();

            if (currentSeries.Any(a => a.Title == series.Title))
            {
                throw new InvalidOperationException($"Title {series.Title} already exists");
            }

            await _context.Series.AddAsync(series);
            await _context.SaveChangesAsync();

            if (thumbnail != null && thumbnail.Length > 0)
            {
                var safeTitle = MyRegex()
                               .Replace(CustomFunction
                               .SanitizeFolderName(series.Title)
                               .Trim().ToLower().Replace(" ", "_"), "");

                var folder = $"uploads/series/{safeTitle}/thumbnail";
                series.Thumbnail = await _fileService.SaveFile(thumbnail, folder, fileName);

                _context.Series.Update(series);
                await _context.SaveChangesAsync();
            }

            series = await _context.Series
                    .Include(s => s.SeriesCategories)
                        .ThenInclude(sc => sc.Category)
                    .Include(s => s.TagCategories)
                        .ThenInclude(tc => tc.Tag)
                    .FirstAsync(s => s.Id == series.Id);

            return series;
        }


        public async Task<Series?> DeleteSeries(int id)
        {
            var filePaths = new List<string>();

            var series = await _context.Series
                .Include(s => s.Episodes)
                .ThenInclude(e => e.Video)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (series == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(series.Thumbnail))
            {
                filePaths.Add(series.Thumbnail);
            }

            foreach (var episode in series.Episodes)
            {
                if (!string.IsNullOrWhiteSpace(episode.Thumbnail))
                    filePaths.Add(episode.Thumbnail);

                if (episode.Video != null && !string.IsNullOrWhiteSpace(episode.Video.VideoUrl))
                    filePaths.Add(episode.Video.VideoUrl);
            }

            _fileService.DeleteFiles(filePaths);
            _context.Series.Remove(series);
            await _context.SaveChangesAsync();
            return series;

        }

        public async Task<List<Series>> GetAllSeries(
            SeriesQueryObject queryObject,
            PaginationQueryObject pagination
        )
        {
            var query = _context.Series
                .Include(s => s.SeriesCategories)
                    .ThenInclude(sc => sc.Category)
                .Include(s => s.TagCategories)
                    .ThenInclude(sc => sc.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Category))
            {
                query = query.Where(s =>
                        s.SeriesCategories.Any(sc =>
                        sc.Category.Name == queryObject.Category)
                        );
            }

            var skipNumber = (pagination.PageNumber - 1) * pagination.PageSize;

            return await query.Skip(skipNumber).Take(pagination.PageSize).ToListAsync();
        }

        public async Task<Series?> GetSeries(string slug)
        {
            var series = await _context.Series
            .Include(e => e.SeriesCategories)
                 .ThenInclude(sc => sc.Category)
            .Include(s => s.TagCategories)
                    .ThenInclude(sc => sc.Tag)
            .Include(e => e.Episodes)
            .Include(c => c.Comments)
            .FirstOrDefaultAsync(i => i.Slug == slug);

            // if (series != null && series.Episodes != null)
            // {
            //     series.Episodes = series.Episodes
            //         .OrderBy(e => e.Season ?? int.MaxValue) // if Season is null, put it last
            //         .ThenBy(e => e.EpisodeNumber ?? int.MaxValue) // if EpisodeNumber is null, put it last
            //         .ToList();
            // }
            // string key = $"slug-{slug}";

            //Icached memory implementation
            // return await _memoryCache.GetOrCreateAsync(key, async entry =>
            // {
            //     entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

            //     var series = await _context.Series
            //         .Include(e => e.SeriesCategories)
            //             .ThenInclude(sc => sc.Category)
            //         .Include(s => s.TagCategories)
            //             .ThenInclude(sc => sc.Tag)
            //         .Include(e => e.Episodes)
            //         .Include(c => c.Comments)
            //             .FirstOrDefaultAsync(i => i.Slug == slug);

            //     return series;
            // });

            return series;
        }

        public async Task<Series?> UpdateSeries(int id, CreateUpdateSeriesDto dto, Stream? thumbnailStream = null, string? thumbnailFileName = null)
        {
            var existingSeries = await _context
                .Series
                // .Include(s => s.SeriesCategories), we can remove this bcs in series without extending to dto already gets all relationship 
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingSeries == null)
            {
                return null;
            }

            existingSeries.Title = dto.Title;
            existingSeries.Description = dto.Description;

            var currentSeries = await _context.Series.ToListAsync();

            if (currentSeries
                    .Any(a => a.Id != existingSeries.Id &&
              a.Title.Trim().Equals(existingSeries.Title.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Title '{dto.Title}' already exists");
            }

            if (thumbnailStream != null && thumbnailStream.Length > 0 && !string.IsNullOrWhiteSpace(thumbnailFileName))
            {
                var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : Directory.GetCurrentDirectory();

                var uploadsFolder = Path.Combine(basePath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(thumbnailFileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await thumbnailStream.CopyToAsync(stream);

                existingSeries.Thumbnail = $"/uploads/{uniqueFileName}";
            }


            //remove categories and tags
            var categoriesToRemove = existingSeries.SeriesCategories
                    .Where(sc => !dto.CategoryIds.Contains(sc.CategoryId))
                    .ToList();

            foreach (var categoryToRemove in categoriesToRemove)
            {
                _context.Remove(categoryToRemove);
            }

            var tagsToRemove = existingSeries.TagCategories
                    .Where(sc => !dto.TagCategoryIds.Contains(sc.TagId))
                    .ToList();

            foreach (var tagToRemove in tagsToRemove)
            {
                _context.Remove(tagToRemove);
            }

            //add categories and tags
            foreach (var categoryId in dto.CategoryIds)
            {

                if (!existingSeries.SeriesCategories.Any(sc => sc.CategoryId == categoryId))
                {
                    existingSeries.SeriesCategories.Add(new SeriesCategory
                    {
                        CategoryId = categoryId,
                        SeriesId = existingSeries.Id
                    });
                }
            }

            foreach (var tagId in dto.TagCategoryIds)
            {

                if (!existingSeries.TagCategories.Any(sc => sc.TagId == tagId))
                {
                    existingSeries.TagCategories.Add(new TagCategory
                    {
                        TagId = tagId,
                        SeriesId = existingSeries.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
            return existingSeries;
        }

        [GeneratedRegex(@"[^a-z0-9_]")]
        private static partial Regex MyRegex();
    }
}