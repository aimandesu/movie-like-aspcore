using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Episode;

namespace application.Dtos.Series
{
    public class SeasonDto
    {
        public int? SeasonNumber { get; set; } // null for movies
        public List<EpisodeDto> Episodes { get; set; } = new();
    }
}