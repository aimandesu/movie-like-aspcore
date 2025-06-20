using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using domain.Entities;
using MediatR;

namespace application.Features.CommentFeature.Delete
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentRequest, Comment?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserService _userService;

        public DeleteCommentHandler(
            IUnitOfWork unitOfWork,
            ICommentRepository commentRepository,
            IUserService userService
        )
        {
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
            _userService = userService;
        }

        public async Task<Comment?> Handle(
            DeleteCommentRequest request,
            CancellationToken cancellationToken
        )
        {
            if (!_userService.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated");

            string? userId = _userService.UserId;

            Comment? comment = await _commentRepository.DeleteComment(
                request.Id, userId ?? "");

            await _unitOfWork.SaveAsync(cancellationToken);

            return comment;

        }
    }
}