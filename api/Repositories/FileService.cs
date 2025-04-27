using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;

namespace api.Repositories
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            string webRootPath;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                webRootPath = _env.WebRootPath; 
            }
            else
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var fullPath = Path.Combine(webRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }


        public void DeleteFiles(IEnumerable<string> relativePaths)
        {
           foreach (var path in relativePaths)
            {
                DeleteFile(path);
            }
        }

        public async Task<string> SaveFile(IFormFile file, string folder)
        {

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var uploadsFolder = Path.Combine(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : Directory.GetCurrentDirectory(),
                folder
            );

            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); 
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{folder}/{uniqueFileName}".Replace("\\", "/");
        }
    }
}