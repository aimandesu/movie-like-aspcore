using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.Features.EpisodeFeature.Delete
{
    public class DeleteEpisodeResponse
    {
        public Episode? Episode { get; set; }
    }
}