using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;
using MediatR;

namespace application.Features.CommentFeature.Delete
{
    public sealed record class DeleteCommentRequest(
        int Id
    ) : IRequest<Comment?>;
}