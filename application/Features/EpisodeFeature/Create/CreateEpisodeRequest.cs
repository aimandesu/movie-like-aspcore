using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Episode;
using MediatR;

namespace application.Features.EpisodeFeature.Create
{
    public sealed record class CreateEpisodeRequest(
        CreateUpdateEpisodeDto CreateUpdateEpisodeDto,
        FileUploadDto Thumbnail,
        FileUploadDto File
    ) : IRequest<CreateEpisodeResponse>;
}