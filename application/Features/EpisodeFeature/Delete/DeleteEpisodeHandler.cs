using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using domain.Entities;
using MediatR;

namespace application.Features.EpisodeFeature.Delete
{
    public class DeleteEpisodeHandler : IRequestHandler<DeleteEpisodeRequest, DeleteEpisodeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IFileService _fileService;

        public DeleteEpisodeHandler(
            IUnitOfWork unitOfWork,
            IEpisodeRepository episodeRepository,
            IFileService fileService
        )
        {
            _unitOfWork = unitOfWork;
            _episodeRepository = episodeRepository;
            _fileService = fileService;
        }

        public async Task<DeleteEpisodeResponse> Handle(
            DeleteEpisodeRequest request,
            CancellationToken cancellationToken
        )
        {

            var filePaths = new List<string>();

            Episode? episode = await _episodeRepository.GetEpisode(request.Id);

            if (episode == null)
            {
                return new DeleteEpisodeResponse
                {
                    Episode = null
                };
            }

            if (!string.IsNullOrWhiteSpace(episode.Thumbnail))
            {
                filePaths.Add(episode.Thumbnail);
            }

            if (episode.Video != null && !string.IsNullOrWhiteSpace(episode.Video.VideoUrl))
            {
                filePaths.Add(episode.Video.VideoUrl);
            }

            _fileService.DeleteFiles(filePaths);

            var episodeDeleted = _episodeRepository.DeleteEpisode(
                episode
            );

            await _unitOfWork.SaveAsync(cancellationToken);

            return new DeleteEpisodeResponse
            {
                Episode = episodeDeleted
            };


        }
    }
}