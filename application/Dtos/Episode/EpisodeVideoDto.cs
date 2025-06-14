using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Video;

namespace application.Dtos.Episode
{
    public class EpisodeVideoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? Season { get; set; } //we have seasons means we are series
        public int? EpisodeNumber { get; set; } //we have episode number, we need to change type in series idk how lol
        public int SeriesId { get; set; }
        public VideoDto? Video { get; set; }
    }
}