using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.IRepository
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllTags();
        Task<Tag?> GetTag(int id);
        Task<Tag?> CreateTag(Tag tag);
        Task<Tag?> DeleteTag(int id);
    }
}