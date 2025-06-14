using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using domain.Entities;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;
        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> CreateTag(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteTag(int id)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(e => e.Id == id);

            if (tag == null)
            {
                return null;
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<List<Tag>> GetAllTags()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag?> GetTag(int id)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}