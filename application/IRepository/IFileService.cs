using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace application.IRepository
{
    public interface IFileService
    {
        Task<string> SaveFile(
            Stream file,
            string folder,
            string originalFileName
        );
        void DeleteFile(string relativePath);
        void DeleteFiles(IEnumerable<string> relativePaths);
    }
}