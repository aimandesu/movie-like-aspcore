using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.SeriesCategories
{
    public class SeriesCategoriesDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}