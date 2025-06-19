using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using domain.Entities;
using MediatR;

namespace application.Features.TagFeature.Delete
{
    public class DeleteTagHandler : IRequestHandler<DeleteTagRequest, Tag?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagRepository _tagRepository;

        public DeleteTagHandler(
            IUnitOfWork unitOfWork,
            ITagRepository tagRepository
        )
        {
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
        }

        public async Task<Tag?> Handle(
            DeleteTagRequest request,
            CancellationToken cancellationToken
        )
        {
            Tag? tag = await _tagRepository.DeleteTag(request.Id);

            await _unitOfWork.SaveAsync(cancellationToken);

            return tag;

        }
    }
}