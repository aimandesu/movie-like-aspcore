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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Category?> CreateCategory(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(e => e.Id == id);

            if (category == null)
            {
                return null;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategory(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}