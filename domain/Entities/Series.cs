using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;




namespace domain.Entities
{
    public enum SeriesFormat
    {
        None = 0,
        Single = 1,
        Series = 2
    }


    public class Series
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public SeriesFormat SeriesFormat { get; set; }
        //Navigation property
        public List<SeriesCategory> SeriesCategories { get; set; } = [];
        public List<TagCategory> TagCategories { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<Impression> Impressions { get; set; } = [];
        public List<Wishlist> Wishlists { get; set; } = [];
        public List<Episode> Episodes { get; set; } = [];

    }
}