using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Comment;
using MediatR;

namespace application.Features.CommentFeature.Create
{
    public sealed record class CreateCommentRequest(
        CreateUpdateCommentDto CreateUpdateCommentDto
    ) : IRequest<CreateCommentResponse>;
}