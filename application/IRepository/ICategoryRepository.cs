using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.IRepository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategories();
        Task<Category?> GetCategory(int id);
        Task<Category?> CreateCategory(Category category);
        Task<Category?> DeleteCategory(int id);
    }
}