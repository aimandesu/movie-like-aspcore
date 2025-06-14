using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.TagCategories;
using domain.Entities;

namespace application.Mappers
{
    public static class TagCategoryMapper
    {
        public static TagCategoriesDto ToTagCategoryDto(this TagCategory tagCategory)
        {
            return new TagCategoriesDto
            {
                TagId = tagCategory.Tag?.Id ?? 0,
                Name = tagCategory.Tag?.Name ?? "",
            };
        }
    }
}