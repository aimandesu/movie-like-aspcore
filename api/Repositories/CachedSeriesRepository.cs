using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Series;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace api.Repositories
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

        public Task<Series?> CreateSeries(Series series, IFormFile file)
        {
            return _decorated.CreateSeries(series, file);
        }

        public Task<Series?> DeleteSeries(int id)
        {
            return _decorated.DeleteSeries(id);
        }

        public Task<List<Series>> GetAllSeries(SeriesQueryObject queryObject, PaginationQueryObject pagination)
        {
            return _decorated.GetAllSeries(queryObject, pagination);
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

        public Task<Series?> UpdateSeries(int id, CreateUpdateSeriesDto dto, IFormFile? thumbnail = null)
        {
            return _decorated.UpdateSeries(id, dto, thumbnail);
        }
    }
}