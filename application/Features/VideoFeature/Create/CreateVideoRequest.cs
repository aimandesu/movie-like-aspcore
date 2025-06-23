using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Video;
using MediatR;

namespace application.Features.VideoFeature.Create
{
    public sealed record class CreateVideoRequest(
        CreateUpdateVideoDto CreateUpdateVideoDto,
        FileUploadDto Thumbnail
    ) : IRequest<CreateVideoResponse>;
}