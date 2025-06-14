using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using domain.Entities;

namespace application.IRepository
{
    public interface ISeriesRepository
    {
        Task<List<Series>> GetAllSeries(
            SeriesQueryObject queryObject,
            PaginationQueryObject pagination
        );
        Task<Series?> GetSeries(string slug);
        Task<Series?> CreateSeries(
            Series series,
            Stream file,
            string fileName
        );
        Task<Series?> DeleteSeries(int id);
        Task<Series?> UpdateSeries(
            int id,
            CreateUpdateSeriesDto dto,
            Stream? thumbnail = null,
            string? thumbnailFileName = null
        );
    }
}