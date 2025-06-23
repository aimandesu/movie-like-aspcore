using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace application.Features.EpisodeFeature.Delete
{
    public sealed record class DeleteEpisodeRequest(
        int Id
    ) : IRequest<DeleteEpisodeResponse>;
}