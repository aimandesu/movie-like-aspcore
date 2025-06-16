using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using MediatR;

namespace application.Features.SeriesFeature.Delete
{
    public class DeleteSeriesHandler : IRequestHandler<DeleteSeriesRequest, DeleteSeriesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISeriesRepository _seriesRepository;


        public DeleteSeriesHandler(
            IUnitOfWork unitOfWork,
            ISeriesRepository seriesRepository,
            IFileService fileService
        )
        {
            _unitOfWork = unitOfWork;
            _seriesRepository = seriesRepository;

        }

        public async Task<DeleteSeriesResponse> Handle(
            DeleteSeriesRequest request,
            CancellationToken cancellationToken
        )
        {
            var series = await _seriesRepository.DeleteSeries(request.Id);

            if (series == null)
                throw new InvalidOperationException("Series not found");

            await _unitOfWork.SaveAsync(cancellationToken);

            return new DeleteSeriesResponse
            {
                Series = series
            };

        }
    }
}