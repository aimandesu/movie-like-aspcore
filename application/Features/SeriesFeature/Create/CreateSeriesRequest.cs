using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using MediatR;

namespace application.Features.SeriesFeature.Create
{
    public sealed record class CreateSeriesRequest(
        CreateUpdateSeriesDto CreateUpdateSeriesDto,
        FileUploadDto Thumbnail
        ) : IRequest<CreateSeriesResponse>;
}