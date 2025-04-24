using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.TagCategories
{
    public class TagCategoriesDto
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int SeriesId { get; set; }
    }
}