using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.TagCategories
{
    public class TagCategoriesDto
    {
        public int TagId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}