using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using MediatR;

namespace application.Features.SeriesFeature.Update
{
    public sealed record class UpdateSeriesRequest(
        int Id,
        CreateUpdateSeriesDto CreateUpdateSeriesDto,
        FileUploadDto? Thumbnail
    ) : IRequest<UpdateSeriesResponse>;
}