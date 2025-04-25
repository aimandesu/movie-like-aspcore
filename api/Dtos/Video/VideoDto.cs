using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Video
{
    public class VideoDto
    {
        public int Id { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        // public string Description { get; set; } = string.Empty;
        // public string Thumbnail { get; set; } = string.Empty;
        public int Duration { get; set; }
        //about metadata? unsure
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public int EpisodeId { get; set; }
    }
}