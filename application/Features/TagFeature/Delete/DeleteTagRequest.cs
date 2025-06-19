using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;
using MediatR;

namespace application.Features.TagFeature.Delete
{
    public sealed record class DeleteTagRequest(
        int Id
    ) : IRequest<Tag?>;
}