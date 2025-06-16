using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace application.Features.SeriesFeature.Delete
{
    public sealed record class DeleteSeriesRequest(
        int Id
    ) : IRequest<DeleteSeriesResponse>;
}