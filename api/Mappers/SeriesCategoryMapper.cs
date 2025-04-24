using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.SeriesCategories;
using api.Models;

namespace api.Mappers
{
    public static class SeriesCategoryMapper
    {
        public static SeriesCategoriesDto ToSeriesCategoryDto(this SeriesCategory seriesCategory)
        {
            return new SeriesCategoriesDto
            {
                Id = seriesCategory.Id,
                CategoryId = seriesCategory.CategoryId,
                SeriesId =  seriesCategory.SeriesId
            };
        }
    }
}