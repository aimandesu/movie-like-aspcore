using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.SeriesCategories;
using api.Models;

namespace api.Dtos.Series
{
    public class SeriesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public SeriesFormat SeriesFormat { get; set; }
        public List<SeriesCategoriesDto> SeriesCategories { get; set; } = [];
        public List<TagCategory> TagCategories { get; set; } = [];
    }
}