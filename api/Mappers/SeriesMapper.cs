using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Series;
using api.Models;

namespace api.Mappers
{
    public static class SeriesMapper
    {
        public static SeriesDto ToSeriesDto (this Series series)
        {
            return new SeriesDto
            {
                Id = series.Id,
                Title = series.Title,
                Description = series.Description,
                Thumbnail = series.Thumbnail,
                CreatedAt = series.CreatedAt,
                SeriesFormat = series.SeriesFormat,
                SeriesCategories = [.. series.SeriesCategories.Select(sc => sc.ToSeriesCategoryDto())],
                TagCategories = series.TagCategories
            };
        }
    }
}