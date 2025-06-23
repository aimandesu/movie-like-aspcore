using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;
using MediatR;

namespace application.Features.VideoFeature.Delete
{
    public sealed record class DeleteVideoRequest(
        int Id
    ) : IRequest<Video?>;
}