using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Episode;
using AutoMapper;
using domain.Entities;

namespace application.Features.EpisodeFeature.Create
{
    public class CreateEpisodeMapper : Profile
    {
        public CreateEpisodeMapper()
        {
            CreateMap<Episode, EpisodeDto>();
            CreateMap<EpisodeDto, Episode>();

        }
    }
}