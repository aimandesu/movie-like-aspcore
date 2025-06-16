using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using application.IRepository;
using application.Mappers;
using AutoMapper;
using domain.Entities;
using MediatR;

namespace application.Features.SeriesFeature.Create
{
    public class CreateSeriesHandler : IRequestHandler<CreateSeriesRequest, CreateSeriesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISeriesRepository _seriesRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public CreateSeriesHandler(
            IUnitOfWork unitOfWork,
            ISeriesRepository seriesRepository,
            IFileService fileService,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _seriesRepository = seriesRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<CreateSeriesResponse> Handle(
            CreateSeriesRequest request,
            CancellationToken cancellationToken
        )
        {
            var dto = request.CreateUpdateSeriesDto;
            var thumbnail = request.Thumbnail;

            // Check for duplicate
            var existing = await _seriesRepository.GetByTitleAsync(dto.Title);
            if (existing != null)
                throw new InvalidOperationException($"Title {dto.Title} already exists");

            var series = new Series
            {
                Title = dto.Title,
                Description = dto.Description,
                Thumbnail = string.Empty, // placeholder
                Slug = CustomFunction.GenerateSlug(dto.Title),
                SeriesFormat = SeriesFormat.None,
                CreatedAt = DateTime.UtcNow,
                SeriesCategories = dto.CategoryIds.Select(catId => new SeriesCategory { CategoryId = catId }).ToList(),
                TagCategories = dto.TagCategoryIds.Select(tagId => new TagCategory { TagId = tagId }).ToList()
            };

            await _seriesRepository.AddAsync(series);
            await _unitOfWork.SaveAsync(cancellationToken); // Save to get ID

            if (thumbnail != null && thumbnail.FileStream.Length > 0)
            {
                var safeTitle = CustomFunction.SanitizeFolderName(dto.Title).ToLower().Replace(" ", "_");
                var folder = $"uploads/series/{safeTitle}/thumbnail";
                var thumbnailPath = await _fileService.SaveFile(thumbnail.FileStream, folder, thumbnail.FileName);

                series.Thumbnail = thumbnailPath;
                _seriesRepository.Update(series);

                await _unitOfWork.SaveAsync(cancellationToken); // Save thumbnail path
            }

            // var loadedSeries = await _seriesRepository.GetWithIncludesAsync(series.Id);

            return new CreateSeriesResponse
            {
                SeriesDto = _mapper.Map<SeriesDto>(series)
            };

        }
    }
}