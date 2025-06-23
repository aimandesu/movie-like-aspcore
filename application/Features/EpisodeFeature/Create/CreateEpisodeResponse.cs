using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Episode;

namespace application.Features.EpisodeFeature.Create
{
    public class CreateEpisodeResponse
    {
        public EpisodeDto? EpisodeDto { get; set; }
    }
}