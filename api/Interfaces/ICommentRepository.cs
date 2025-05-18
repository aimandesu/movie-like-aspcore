using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;

namespace api.Interfaces
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