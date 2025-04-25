using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file, string folder);
        void DeleteFile(string relativePath);
        void DeleteFiles(IEnumerable<string> relativePaths);
    }
}