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

        public async Task<Series?> GetByIdAsync(
            int id,
            bool includeEpisode = false
        )
        {
            var query = _context.Series
                .Include(s => s.SeriesCategories)
                .Include(s => s.TagCategories)
                .AsQueryable();

            if (includeEpisode)
            {
                query = query.Include(s => s.Episodes);
            }

            return await query.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsByTitleAsync(string title, int? excludeId = null)
        {
            return await _context.Series.AnyAsync(s =>
                s.Title.Trim().ToLower() == title.Trim().ToLower()
                && (!excludeId.HasValue || s.Id != excludeId));
        }

        public async Task<Series?> GetByTitleAsync(string title)
        {
            return await _context.Series
                .FirstOrDefaultAsync(s => s.Title == title);
        }

        public async Task AddAsync(Series series)
        {
            await _context.Series.AddAsync(series);
        }

        public void Update(Series series)
        {
            _context.Series.Update(series);
        }

        // public async Task<Series> GetWithIncludesAsync(int id)
        // {
        //     return await _context.Series
        //         .Include(s => s.SeriesCategories).ThenInclude(sc => sc.Category)
        //         .Include(s => s.TagCategories).ThenInclude(tc => tc.Tag)
        //         .FirstAsync(s => s.Id == id);
        // }
    }
}