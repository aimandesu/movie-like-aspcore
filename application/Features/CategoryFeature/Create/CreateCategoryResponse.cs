using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.Features.CategoryFeature.Create
{
    public sealed record CreateCategoryResponse
    {
        public Category? Category { get; set; }
    }
}