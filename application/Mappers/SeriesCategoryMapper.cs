using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.SeriesCategories;
using domain.Entities;

namespace application.Mappers
{
    public static class SeriesCategoryMapper
    {
        public static SeriesCategoriesDto ToSeriesCategoryDto(this SeriesCategory seriesCategory)
        {
            return new SeriesCategoriesDto
            {
                Name = seriesCategory.Category?.Name ?? "",
                CategoryId = seriesCategory.Category?.Id ?? 0,
            };
        }
    }
}