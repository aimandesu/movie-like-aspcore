using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? Season { get; set; }
        public int? EpisodeNumber { get; set; }
        public int SeriesId { get; set; }
        required public Series Series { get; set; }
         public int VideoId { get; set; }
        required public Video Video { get; set; }
        
    }
}