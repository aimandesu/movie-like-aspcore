using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Category;
using domain.Entities;
using MediatR;

namespace application.Features.CategoryFeature.Create
{
    public sealed record class CreateCategoryRequest(
        CreateCategoryDto CategoryDto
    ) : IRequest<CreateCategoryResponse>;
}