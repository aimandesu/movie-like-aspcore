using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;

namespace api.Dtos.Series
{
    public class CreateUpdateSeriesDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        // [Required]
        public string Thumbnail { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        // [Required]
        [NotEmptyList]
        public List<int> CategoryIds { get; set; } = [];

        [NotEmptyList]
        public List<int> TagCategoryIds { get; set; } = [];
    }
}