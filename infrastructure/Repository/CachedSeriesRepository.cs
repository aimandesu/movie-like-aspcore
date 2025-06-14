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

        public Task<Series?> CreateSeries(
            Series series,
            Stream file,
            string fileName
        )
        {
            return _decorated.CreateSeries(
                series,
                file,
                fileName
            );
        }

        public Task<Series?> DeleteSeries(int id)
        {
            return _decorated.DeleteSeries(id);
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

        public Task<Series?> UpdateSeries(
            int id,
            CreateUpdateSeriesDto dto,
            Stream? thumbnail = null,
            string? thumbnailFileName = null
        )
        {
            return _decorated.UpdateSeries(id, dto, thumbnail);
        }
    }
}