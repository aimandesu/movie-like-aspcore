using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using application.IRepository;
using AutoMapper;
using domain.Entities;
using MediatR;

namespace application.Features.SeriesFeature.Update
{
    public class UpdateSeriesHandler : IRequestHandler<UpdateSeriesRequest, UpdateSeriesResponse>
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSeriesHandler(
            ISeriesRepository seriesRepository,
            IFileService fileService,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _seriesRepository = seriesRepository;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UpdateSeriesResponse> Handle(
            UpdateSeriesRequest request,
            CancellationToken cancellationToken
        )
        {
            var id = request.Id;
            var dto = request.CreateUpdateSeriesDto;
            var thumbnail = request.Thumbnail;

            var existingSeries = await _seriesRepository.GetByIdAsync(id);

            if (existingSeries == null)
            {
                throw new InvalidOperationException($"Series with ID {id} not found.");
                // throw new NotFoundException($"Series with ID {id} not found.");
            }

            // Title uniqueness validation
            if (await _seriesRepository.ExistsByTitleAsync(dto.Title, excludeId: id))
            {
                throw new InvalidOperationException($"Title '{dto.Title}' already exists.");
            }

            // Update fields
            existingSeries.Title = dto.Title;
            existingSeries.Description = dto.Description;

            // Update thumbnail if exists
            if (thumbnail?.FileStream != null && thumbnail.FileStream.Length > 0)
            {
                var safeTitle = CustomFunction.SanitizeFolderName(dto.Title).ToLower().Replace(" ", "_");
                var folder = $"uploads/series/{safeTitle}/thumbnail";
                var savedPath = await _fileService.SaveFile(
                    thumbnail.FileStream, folder, thumbnail.FileName
                );
                //delete existing path
                _fileService.DeleteFile(existingSeries.Thumbnail);

                //add new path
                existingSeries.Thumbnail = savedPath;

            }

            // Update SeriesCategories
            existingSeries.SeriesCategories.RemoveAll(sc => !dto.CategoryIds.Contains(sc.CategoryId));
            foreach (var categoryId in dto.CategoryIds)
            {
                if (!existingSeries.SeriesCategories.Any(sc => sc.CategoryId == categoryId))
                {
                    existingSeries.SeriesCategories.Add(new SeriesCategory
                    {
                        CategoryId = categoryId,
                        SeriesId = existingSeries.Id
                    });
                }
            }

            // Update TagCategories
            existingSeries.TagCategories.RemoveAll(tc => !dto.TagCategoryIds.Contains(tc.TagId));
            foreach (var tagId in dto.TagCategoryIds)
            {
                if (!existingSeries.TagCategories.Any(tc => tc.TagId == tagId))
                {
                    existingSeries.TagCategories.Add(new TagCategory
                    {
                        TagId = tagId,
                        SeriesId = existingSeries.Id
                    });
                }
            }

            await _unitOfWork.SaveAsync(cancellationToken);

            return new UpdateSeriesResponse
            {
                SeriesDto = _mapper.Map<SeriesDto>(existingSeries)
            };
        }
    }

}