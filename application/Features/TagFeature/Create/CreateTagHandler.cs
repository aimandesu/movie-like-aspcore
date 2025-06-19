using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using AutoMapper;
using domain.Entities;
using MediatR;

namespace application.Features.TagFeature.Create
{
    public class CreateTagHandler : IRequestHandler<CreateTagRequest, CreateTagResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public CreateTagHandler(
            IUnitOfWork unitOfWork,
            ITagRepository tagRepository,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<CreateTagResponse> Handle(
            CreateTagRequest request,
            CancellationToken cancellationToken
        )
        {
            Tag? tag = _mapper.Map<Tag>(request.CreateTagDto);

            tag = await _tagRepository.CreateTag(tag);

            await _unitOfWork.SaveAsync(cancellationToken);

            return new CreateTagResponse
            {
                Tag = tag
            };

        }
    }
}