using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Category;
using application.IRepository;
using AutoMapper;
using domain.Entities;
using MediatR;

namespace application.Features.CategoryFeature.Create
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryRequest, CreateCategoryResponse>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryHandler(
            IUnitOfWork unitOfWork,
            ICategoryRepository categoryRepository,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CreateCategoryResponse> Handle(
            CreateCategoryRequest request,
            CancellationToken cancellationToken
        )
        {
            //map here
            var categoryMapper = _mapper.Map<Category>(request.CategoryDto);

            Category? category = await _categoryRepository.CreateCategory(categoryMapper);

            await _unitOfWork.SaveAsync(cancellationToken);

            return new CreateCategoryResponse
            {
                Category = category
            };

        }
    }
}