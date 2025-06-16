using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using application.IRepository;
using domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace infrastructure.Repository
{
    public class CachedSeriesRepository : ISeriesRepository
    {

        private readonly SeriesRepository _decorated;
        private readonly IMemoryCache _memoryCache;

        public CachedSeriesRepository(
            SeriesRepository decorated,
            IMemoryCache memoryCache
        )
        {
            _decorated = decorated;
            _memoryCache = memoryCache;
        }

        public Task AddAsync(Series series)
        {
            return _decorated.AddAsync(series);
        }

        public Task<Series?> DeleteSeries(int id)
        {
            return _decorated.DeleteSeries(id);
        }

        public Task<bool> ExistsByTitleAsync(string title, int? excludeId = null)
        {
            return _decorated.ExistsByTitleAsync(title, excludeId);
        }

        public Task<List<Series>> GetAllSeries(
            SeriesQueryObject queryObject,
            PaginationQueryObject pagination
        )
        {
            return _decorated.GetAllSeries(
                queryObject,
                pagination
            );
        }

        public Task<Series?> GetByIdAsync(int id)
        {
            return _decorated.GetByIdAsync(id);
        }

        public Task<Series?> GetByTitleAsync(string title)
        {
            return _decorated.GetByTitleAsync(title);
        }

        public Task<Series?> GetSeries(string slug)
        {
            string key = $"slug-{slug}";

            return _memoryCache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                    return _decorated.GetSeries(slug);
                }
            );
        }

        // public Task<Series> GetWithIncludesAsync(int id)
        // {
        //     return _decorated.GetWithIncludesAsync(id);
        // }

        public void Update(Series series)
        {
            _decorated.Update(series);
        }
    }
}