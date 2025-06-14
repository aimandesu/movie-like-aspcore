using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Video;
using application.IRepository;
using domain.Entities;
using infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;
        public VideoRepository(
            ApplicationDbContext context,
            IFileService fileService,
            IWebHostEnvironment env
        )
        {
            _context = context;
            _env = env;
            _fileService = fileService;
        }
        public async Task<Video?> CreateVideo(
            Video video,
            Stream file,
            string fileName
        )
        {

            var episode = await _context.Episodes
                        .Include(s => s.Series)
                        .FirstOrDefaultAsync(e => e.Id == video.EpisodeId);

            if (episode == null) return null;

            await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();

            var safeTitle = Regex.Replace(
                CustomFunction.SanitizeFolderName(episode.Series?.Title ?? "")
                    .Trim()
                    .ToLower()
                    .Replace(" ", "_"),
                @"[^a-z0-9_]", ""
            );


            if (file != null && file.Length > 0)
            {
                var videoFolder = $"uploads/series/{safeTitle}/video";
                var videoPath = await _fileService.SaveFile(file, videoFolder, fileName);

                // update the existing instance
                video.VideoUrl = videoPath;
                video.Duration = 0;
                video.UpdatedAt = DateTime.UtcNow;
                video.ViewCount = 0;

                // _context.Videos.Update(video); // optional: EF may already track it
                await _context.SaveChangesAsync();
            }


            return video;
        }

        public async Task<Video?> DeleteVideo(int id)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.Id == id);

            if (video == null)
            {
                return null;
            }

            if (video != null && !string.IsNullOrWhiteSpace(video.VideoUrl))
            {
                _fileService.DeleteFile(video.VideoUrl);
                _context.Videos.Remove(video);
            }

            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<Video?> GetVideo(int id)
        {
            return await _context.Videos
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // public async Task StreamVideo(
        //     int id,
        //     Stream outputStream,
        //     HttpRequest request,
        //     HttpResponse response
        // )
        // {
        //     var video = await _context.Videos.FirstOrDefaultAsync(e => e.Id == id);

        //     // Determine web root path based on environment
        //     string filepath = video?.VideoUrl ?? "";

        //     var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
        //        ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
        //        : Directory.GetCurrentDirectory();

        //     // Create full path by combining web root with the relative path
        //     var fullPath = Path.Combine(basePath, filepath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        //     // Ensure the file exists
        //     if (string.IsNullOrEmpty(filepath) || !System.IO.File.Exists(fullPath))
        //     {
        //         response.StatusCode = 404;
        //         return;
        //     }

        //     string filename = Path.GetFileName(fullPath);
        //     Stream iStream = null;
        //     byte[] buffer = new byte[4096];
        //     int bytesRead;
        //     long totalBytes;

        //     try
        //     {
        //         // Open the file - using fullPath
        //         iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        //         // Total bytes to read
        //         totalBytes = iStream.Length;

        //         // Set the proper content type for MP4 files
        //         response.ContentType = "video/mp4";
        //         response.Headers["Accept-Ranges"] = "bytes";

        //         // Handle range requests for video seeking
        //         long startByte = 0;
        //         long endByte = totalBytes - 1;

        //         if (!string.IsNullOrEmpty(request.Headers["Range"]))
        //         {
        //             string rangeHeader = request.Headers["Range"].ToString();
        //             string[] rangeParts = rangeHeader.Replace("bytes=", "").Split('-');

        //             if (rangeParts.Length > 0 && !string.IsNullOrEmpty(rangeParts[0]))
        //             {
        //                 startByte = long.Parse(rangeParts[0]);
        //             }

        //             if (rangeParts.Length > 1 && !string.IsNullOrEmpty(rangeParts[1]))
        //             {
        //                 endByte = long.Parse(rangeParts[1]);
        //             }

        //             // Seek to the requested position
        //             iStream.Seek(startByte, SeekOrigin.Begin);

        //             // Set partial content response
        //             response.StatusCode = 206;
        //             response.Headers["Content-Range"] = $"bytes {startByte}-{endByte}/{totalBytes}";
        //             response.Headers["Content-Length"] = (endByte - startByte + 1).ToString();
        //         }
        //         else
        //         {
        //             // Full content response
        //             response.Headers["Content-Length"] = totalBytes.ToString();
        //         }

        //         long bytesToRead = endByte - startByte + 1;

        //         // Stream the file
        //         while (bytesToRead > 0)
        //         {
        //             // Check if the client is still connected
        //             if (request.HttpContext.RequestAborted.IsCancellationRequested)
        //                 break;

        //             // Read data into buffer
        //             int readSize = (int)Math.Min(buffer.Length, bytesToRead);
        //             bytesRead = await iStream.ReadAsync(buffer, 0, readSize);

        //             if (bytesRead == 0)
        //                 break; // End of file

        //             // Write to output stream
        //             await outputStream.WriteAsync(buffer, 0, bytesRead);
        //             await outputStream.FlushAsync();

        //             // Update bytes remaining
        //             bytesToRead -= bytesRead;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log the exception
        //         Console.WriteLine($"Error streaming video: {ex.Message}");
        //         response.StatusCode = 500;
        //     }
        //     finally
        //     {
        //         // Close the file
        //         iStream?.Close();
        //     }
        // }

        public Task<Video?> UpdateVideo(
            int id,
            CreateUpdateVideoDto dto,
            Stream? video
        )
        {
            throw new NotImplementedException();
        }
    }
}