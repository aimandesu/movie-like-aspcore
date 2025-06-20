using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using AutoMapper;
using domain.Entities;
using MediatR;

namespace application.Features.CommentFeature.Create
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentRequest, CreateCommentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CreateCommentHandler(
            IUnitOfWork unitOfWork,
            ICommentRepository commentRepository,
            IUserService userService,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<CreateCommentResponse> Handle(
            CreateCommentRequest request,
            CancellationToken cancellationToken
        )
        {
            if (!_userService.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated");

            string userId = _userService?.UserId ?? "";

            Comment comment = _mapper.Map<Comment>(request.CreateUpdateCommentDto);

            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;

            comment = await _commentRepository.AddComment(comment);
            await _unitOfWork.SaveAsync(cancellationToken);

            return new CreateCommentResponse
            {
                Comment = comment
            };

        }
    }
}