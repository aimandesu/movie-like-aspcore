using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Episode;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class EpisodeRepository : IEpisodeRepository
    {
        private readonly ApplicationDbContext _context;
        public EpisodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Episode?> CreateEpisode(
            Episode episode, 
            IFormFile thumbnail, 
            IFormFile file
        )
        {
            if (thumbnail != null && thumbnail.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder); 

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnail.FileName); 
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await thumbnail.CopyToAsync(stream);

                episode.Thumbnail = $"/uploads/{uniqueFileName}"; 
            }

            if (file != null && file.Length > 0)
            {
              
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos"); // adjust to your project structure
                Directory.CreateDirectory(uploadsFolder); 

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); 
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var video = new Video
                {
                    VideoUrl = $"/videos/{uniqueFileName}", 
                    Duration = 0, 
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ViewCount = 0,
                    Episode = null
                };
          
                // episode.Series = await _context.Series.FindAsync(episode.SeriesId); //kena ke ni
                video.Episode = episode;
                _context.Videos.Add(video);
                _context.Episodes.Add(episode);
                }
               
                await _context.SaveChangesAsync();

                return episode;
        }


        public Task<Episode?> DeleteEpisode(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Episode>> GetAllEpisodes(EpisodeQueryObject queryObject)
        {
            throw new NotImplementedException();

            // return await _context.Episodes
            //     .Include(e => e.Series)
            //     .Where(e => e.Series.Title.ToLower() == queryObject.SeriesTitle.ToLower())
            //     .ToListAsync();

            // var query = _context.Episodes
            //     .Include(e => e.Series)
            //     .AsQueryable();

            // if (!string.IsNullOrWhiteSpace(queryObject.SeriesTitle))
            // {
            //     query = query.Where(e =>
            //         e.Series.Title.ToLower() == queryObject.SeriesTitle.ToLower());
            // }

            // var episodes = await query.ToListAsync();

            // return episodes;
        }

        public async Task<Episode?> GetEpisode(int id)
        {
            return await _context
                .Episodes.FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<Series?> UpdateSeries(int id, CreateUpdateEpisodeDto dto, IFormFile? thumbnail = null)
        {
            throw new NotImplementedException();
        }
    }
}