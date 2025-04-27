using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Series;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
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

        public async Task<Series?> CreateSeries(Series series, IFormFile thumbnail)
        {
            await _context.Series.AddAsync(series);
            await _context.SaveChangesAsync();

            if (thumbnail != null && thumbnail.Length > 0)
            {
                 var safeTitle = MyRegex()
                                .Replace(CustomFunction
                                .SanitizeFolderName(series.Title)
                                .Trim().ToLower().Replace(" ", "_"), "");

                var folder = $"uploads/series/{safeTitle}/thumbnail";
                series.Thumbnail = await _fileService.SaveFile(thumbnail, folder);

                _context.Series.Update(series);
                await _context.SaveChangesAsync();
            }

            return series;
        }


        public async Task<Series?> DeleteSeries(int id)
        {
            var filePaths = new List<string>();

            var series = await _context.Series
                .Include(s => s.Episodes)
                .ThenInclude(e => e.Video)
                .FirstOrDefaultAsync(e => e.Id == id);

            if(series == null)
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

        public async Task<List<Series>> GetAllSeries(SeriesQueryObject queryObject)
        {
            var query = _context.Series
                // .Include(s => s.SeriesCategories)
                // .ThenInclude(sc => sc.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Category))
            {
                query = query.Where(s =>
                        s.SeriesCategories.Any(sc => 
                        sc.Category.Name == queryObject.Category)
                        );
            }

            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await query.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Series?> GetSeries(int id)
        {
            return await _context.Series
            // .Include(e => e.SeriesCategories)
            .Include(e => e.Episodes)
            .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Series?> UpdateSeries(int id, CreateUpdateSeriesDto dto, IFormFile? thumbnail = null)
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

            if (thumbnail != null && thumbnail.Length > 0)
            {
                var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : Directory.GetCurrentDirectory();

                var uploadsFolder = Path.Combine(basePath, "uploads");

                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{thumbnail.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await thumbnail.CopyToAsync(stream);

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