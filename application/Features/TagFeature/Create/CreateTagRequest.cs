using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Tag;
using domain.Entities;
using MediatR;

namespace application.Features.TagFeature.Create
{
    public sealed record class CreateTagRequest(
        CreateTagDto CreateTagDto
    ) : IRequest<CreateTagResponse>;
}