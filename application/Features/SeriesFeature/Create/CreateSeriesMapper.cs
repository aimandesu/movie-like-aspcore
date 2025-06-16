using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Series;
using application.Dtos.SeriesCategories;
using application.Dtos.TagCategories;
using AutoMapper;
using domain.Entities;

namespace application.Features.SeriesFeature.Create
{
    public class CreateSeriesMapper : Profile
    {
        public CreateSeriesMapper()
        {
            CreateMap<Series, SeriesDto>();
            CreateMap<SeriesCategory, SeriesCategoriesDto>();
            CreateMap<TagCategory, TagCategoriesDto>();
        }
    }
}