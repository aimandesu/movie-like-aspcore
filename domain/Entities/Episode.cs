using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? Season { get; set; } //we have seasons means we are series
        public int? EpisodeNumber { get; set; } //we have episode number, we need to change type in series idk how lol
        public int SeriesId { get; set; }
        // public int VideoId { get; set; }
        //navigation property
        public Series? Series { get; set; }
        public Video? Video { get; set; }
    }
}