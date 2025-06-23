using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using domain.Entities;
using MediatR;

namespace application.Features.VideoFeature.Delete
{
    public class DeleteVideoHandler : IRequestHandler<DeleteVideoRequest, Video?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoRepository _videoRepository;
        private readonly IFileService _fileService;

        public DeleteVideoHandler(
            IUnitOfWork unitOfWork,
            IVideoRepository videoRepository,
            IFileService fileService
        )
        {
            _unitOfWork = unitOfWork;
            _videoRepository = videoRepository;
            _fileService = fileService;
        }

        public async Task<Video?> Handle(
            DeleteVideoRequest request,
            CancellationToken cancellationToken
        )
        {
            var video = await _videoRepository.DeleteVideo(request.Id);

            if (video?.VideoUrl != null)
            {
                _fileService.DeleteFile(video.VideoUrl);
            }

            await _unitOfWork.SaveAsync(cancellationToken);

            return video;

        }
    }
}