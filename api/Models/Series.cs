using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Series
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int TypeId { get; set; }
        //Navigation property
        required public SeriesType SeriesType { get; set; }
        public List<Comment> Comments { get; set; } = [];
        public List<Impression> Impressions { get; set; } = [];
        public List<Wishlist> Wishlists { get; set; } = [];
        public List<Episode> Episodes { get; set; } = [];

    }
}