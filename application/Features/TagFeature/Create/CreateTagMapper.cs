using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Tag;
using AutoMapper;
using domain.Entities;


namespace application.Features.TagFeature.Create
{
    public class CreateTagMapper : Profile
    {
        public CreateTagMapper()
        {
            CreateMap<CreateTagDto, Tag>();
        }
    }
}