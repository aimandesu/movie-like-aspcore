using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using application.Dtos.Category;
using domain.Entities;

namespace application.IRepository
{
    public interface ICategoryRepository
    {
        Task<Pagination<Category>> GetAllCategories(int page,  int perPage);
        Task<List<CategorySeriesDto>> GetCategory(int id);
        Task<Category?> CreateCategory(Category category);
        Task<Category?> DeleteCategory(int id);
    }
}