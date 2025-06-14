using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using Microsoft.AspNetCore.Hosting;

namespace infrastructure.Repository
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

        public async Task<string> SaveFile(Stream fileStream, string folder, string originalFileName)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is empty.");

            var uploadsFolder = Path.Combine(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : Directory.GetCurrentDirectory(),
                folder
            );

            Directory.CreateDirectory(uploadsFolder);
            var extension = Path.GetExtension(originalFileName);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}"; // or pass in an extension as a param
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return $"/{folder}/{uniqueFileName}".Replace("\\", "/");
        }
    }
}