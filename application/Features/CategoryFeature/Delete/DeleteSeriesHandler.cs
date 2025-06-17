using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using domain.Entities;
using MediatR;

namespace application.Features.CategoryFeature.Delete
{
    public class DeleteSeriesHandler : IRequestHandler<DeleteSeriesRequest, Category?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public DeleteSeriesHandler(
            IUnitOfWork unitOfWork,
            ICategoryRepository categoryRepository
        )
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<Category?> Handle(
            DeleteSeriesRequest request,
            CancellationToken cancellationToken
        )
        {

            Category? category = await _categoryRepository.DeleteCategory(request.Id);

            await _unitOfWork.SaveAsync(cancellationToken);

            return category;

        }
    }
}