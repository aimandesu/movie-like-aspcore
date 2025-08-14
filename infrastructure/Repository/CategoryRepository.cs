using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using application.Dtos.Category;
using application.IRepository;
using application.Mappers;
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
        public async Task<Category?> CreateCategory(
            Category category
        )
        {
            await _context.Categories.AddAsync(category);

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
            return category;
        }

        public async Task<Pagination<Category>> GetAllCategories(int page, int perPage)
        {
            return await _context.Categories
                .PaginatedAsync(page, perPage);
        }

        public async Task<List<CategorySeriesDto>> GetCategory(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .GroupJoin(
                    _context.SeriesCategories,
                    c => c.Id,
                    sc => sc.CategoryId,
                    (c, sc) => new
                    {
                        Category = c,
                        Series = sc
                    })
                .Select(g => CategoryMapper.ToCategorySeriesDto(
                            g.Category,
                            g.Series.Select(sc => sc.Series.ToSeriesDto()).ToList()
                        )

                //     new CategorySeriesDto
                // {
                //     Id = g.Category.Id,
                //     Name = g.Category.Name,
                //     SeriesDto = g.Series
                //         .Select(sc => sc.Series.ToSeriesDto())
                //         .ToList()
                // }
                )
                .ToListAsync();
        }
    }
}