using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Comment;
using domain.Entities;

namespace application.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Discussion = comment.Discussion,
                UserId = comment.UserId,
                Username = comment.User?.UserName ?? "",
                // EpisodeId = comment.EpisodeId,
                CreatedAt = comment.CreatedAt,
            };
        }
    }
}