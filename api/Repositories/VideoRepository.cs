using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Video;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        public VideoRepository(
            ApplicationDbContext context,
            IFileService fileService
        )
        {
            _context = context;
            _fileService = fileService;
        }
        public async Task<Video?> CreateVideo(
            Video video, 
            IFormFile file
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
                var videoPath = await _fileService.SaveFile(file, videoFolder);

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

            if(video == null)
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

        public Task<Video?> UpdateVideo(
            int id, 
            CreateUpdateVideoDto dto, 
            IFormFile? video
        )
        {
            throw new NotImplementedException();
        }
    }
}