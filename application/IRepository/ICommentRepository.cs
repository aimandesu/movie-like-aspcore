using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using domain.Entities;

namespace application.IRepository
{
    public interface ICommentRepository
    {
        Task<Comment> AddComment(Comment comment);
        Task<Comment?> DeleteComment(int id, string username);
        Task<List<Comment>> GetAllComments(
            PaginationQueryObject pagination,
            CommentQueryObject commentQuery,
            int episodeId
        );
        Task<Comment?> GetComment(int id);
    }
}