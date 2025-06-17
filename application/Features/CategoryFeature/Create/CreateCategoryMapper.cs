using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using application.Dtos.Category;
using domain.Entities;

namespace application.Features.CategoryFeature.Create
{
    public class CreateCategoryMapper : Profile
    {
        public CreateCategoryMapper()
        {
            CreateMap<CreateCategoryDto, Category>();
        }
    }
}