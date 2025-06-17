using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;
using MediatR;

namespace application.Features.CategoryFeature.Delete
{
    public sealed record class DeleteSeriesRequest(int Id) : IRequest<Category?>;
}