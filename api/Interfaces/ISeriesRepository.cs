using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Series;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface ISeriesRepository
    {
        Task<List<Series>> GetAllSeries(
            SeriesQueryObject queryObject,
            PaginationQueryObject pagination
        );
        Task<Series?> GetSeries(string slug);
        Task<Series?> CreateSeries(Series series, IFormFile file);
        Task<Series?> DeleteSeries(int id);
        Task<Series?> UpdateSeries(int id, CreateUpdateSeriesDto dto, IFormFile? thumbnail = null);
    }
}