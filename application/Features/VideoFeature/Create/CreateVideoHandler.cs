using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.IRepository;
using domain.Entities;
using MediatR;

namespace application.Features.VideoFeature.Create
{
    public class CreateVideoHandler : IRequestHandler<CreateVideoRequest, CreateVideoResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoRepository _videoRepository;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IFileService _fileService;

        public CreateVideoHandler(
            IUnitOfWork unitOfWork,
            IVideoRepository videoRepository,
            IEpisodeRepository episodeRepository,
            IFileService fileService
        )
        {
            _unitOfWork = unitOfWork;
            _videoRepository = videoRepository;
            _episodeRepository = episodeRepository;
            _fileService = fileService;
        }

        public async Task<CreateVideoResponse> Handle(
            CreateVideoRequest request,
            CancellationToken cancellationToken
        )
        {
            var dto = request.CreateUpdateVideoDto;

            var video = new Video
            {
                VideoUrl = string.Empty,
                Duration = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ViewCount = 0,
                EpisodeId = dto.EpisodeId,
            };

            var episode = await _episodeRepository.GetEpisode(
                request.CreateUpdateVideoDto.EpisodeId
            );

            if (episode == null)
            {
                return new CreateVideoResponse
                {
                    Video = null
                };
            }
            ;

            var safeTitle = CustomFunction.SanitizeFolderName(episode?.Series?.Title ?? "").ToLower().Replace(" ", "_");

            if (request.Thumbnail.FileStream.Length > 0)
            {

                var folder = $"uploads/series/{safeTitle}/video";
                var videoPath = await _fileService.SaveFile(
                    request.Thumbnail.FileStream,
                    folder,
                    request.Thumbnail.FileName
                );

                video.VideoUrl = videoPath;
            }

            var createdVideo = await _videoRepository.CreateVideo(
                video
            );

            await _unitOfWork.SaveAsync(cancellationToken);

            return new CreateVideoResponse
            {
                Video = createdVideo
            };

        }
    }
}