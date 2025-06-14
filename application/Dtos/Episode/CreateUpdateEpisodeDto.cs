using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace application.Dtos.Episode
{
    public class CreateUpdateEpisodeDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public int? Season { get; set; }
        public int? EpisodeNumber { get; set; }

        required public int SeriesId { get; set; }

    }
}