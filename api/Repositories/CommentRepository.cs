using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Comment> AddComment(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> DeleteComment(int id, string username)
        {

            var user = await _context.Users.Include(u => u.Comments)
                .FirstOrDefaultAsync(u => u.UserName == username);

            var comments = user?.Comments ?? [];

            var commentToDelete = comments.FirstOrDefault(c => c.Id == id);

            if (commentToDelete != null)
            {
                _context.Comments.Remove(commentToDelete);
                await _context.SaveChangesAsync();
            }

            return commentToDelete;

        }

        public async Task<List<Comment>> GetAllComments(
            PaginationQueryObject pagination,
            CommentQueryObject commentQuery,
            int episodeId
        )
        {
            var query = _context.Comments
                .Where(c => c.EpisodeId == episodeId)
                .AsQueryable();

            query = commentQuery.IsDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);

            var skipNumber = (pagination.PageNumber - 1) * pagination.PageSize;

            return await query
                .Skip(skipNumber)
                .Take(pagination.PageSize)
                .Include(c => c.User)
                // .Include(c => c.Episode)
                .ToListAsync();
        }

        public async Task<Comment?> GetComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(i => i.Id == id);

            return comment;
        }
    }
}