using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Episode;
using application.IRepository;
using AutoMapper;
using domain.Entities;
using MediatR;

namespace application.Features.EpisodeFeature.Create
{
    public class CreateEpisodeHandler : IRequestHandler<CreateEpisodeRequest, CreateEpisodeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly ISeriesRepository _seriesRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public CreateEpisodeHandler(
            IUnitOfWork unitOfWork,
            IEpisodeRepository episodeRepository,
            ISeriesRepository seriesRepository,
            IFileService fileService,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _episodeRepository = episodeRepository;
            _seriesRepository = seriesRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<CreateEpisodeResponse> Handle(
            CreateEpisodeRequest request,
            CancellationToken cancellationToken
        )
        {
            var episodeDto = request.CreateUpdateEpisodeDto;
            var safeTitle = CustomFunction.SanitizeFolderName(episodeDto.Title).ToLower().Replace(" ", "_");

            var series = await _seriesRepository.GetByIdAsync(
                id: episodeDto.SeriesId,
                includeEpisode: true
            );

            if (series == null)
            {
                throw new KeyNotFoundException($"Series with ID {episodeDto.SeriesId} not found");
            }

            if (episodeDto.Season != null && episodeDto.EpisodeNumber != null && series.Episodes.Any(a =>
                a.Season == episodeDto.Season
                && a.EpisodeNumber == episodeDto.EpisodeNumber
            ))
            {
                throw new InvalidOperationException($"Episode already exists in season {episodeDto.Season} with number {episodeDto.EpisodeNumber}");
            }

            //EPISODE
            var episode = new Episode
            {
                Title = episodeDto.Title,
                Description = episodeDto.Description,
                Thumbnail = string.Empty,
                CreatedAt = DateTime.UtcNow,
                SeriesId = episodeDto.SeriesId,
                Season = episodeDto.Season,
                EpisodeNumber = episodeDto.EpisodeNumber,
            };

            if (request.Thumbnail.FileStream.Length > 0)
            {

                var folder = $"uploads/series/{safeTitle}/episode";
                var thumbnailPath = await _fileService.SaveFile(
                    request.Thumbnail.FileStream,
                    folder,
                    request.Thumbnail.FileName
                );

                episode.Thumbnail = thumbnailPath;
            }

            //VIDEO
            var video = new Video
            {
                VideoUrl = string.Empty,
                Duration = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ViewCount = 0,
                Episode = episode
            };

            if (request.File.FileStream.Length > 0)
            {
                var folder = $"uploads/series/{safeTitle}/video";
                var videoPath = await _fileService.SaveFile(
                    request.File.FileStream,
                    folder,
                    request.File.FileName
                );

                video.VideoUrl = videoPath;
            }

            var episodeCreated = _episodeRepository.CreateEpisode(
                episode,
                video
                );

            await _unitOfWork.SaveAsync(cancellationToken);

            return new CreateEpisodeResponse
            {
                EpisodeDto = _mapper.Map<EpisodeDto>(episodeCreated)
            };

        }
    }
}