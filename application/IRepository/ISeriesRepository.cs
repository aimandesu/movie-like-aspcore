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

        Task<Series?> DeleteSeries(int id);

        Task AddAsync(Series series);
        Task<Series?> GetByTitleAsync(string title);
        // Task<Series> GetWithIncludesAsync(int id);
        void Update(Series series);
        Task<Series?> GetByIdAsync(int id);
        Task<bool> ExistsByTitleAsync(string title, int? excludeId = null);

    }
}