using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Comment;
using AutoMapper;
using domain.Entities;

namespace application.Features.CommentFeature.Create
{
    public class CreateCommentMapper : Profile
    {
        public CreateCommentMapper()
        {
            CreateMap<CreateUpdateCommentDto, Comment>();
        }
    }
}