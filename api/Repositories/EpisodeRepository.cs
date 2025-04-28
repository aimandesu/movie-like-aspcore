using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public partial class EpisodeRepository : IEpisodeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EpisodeRepository> _logger;
        private readonly IFileService _fileService;
        public EpisodeRepository(
            ApplicationDbContext context,
            IFileService fileService,
            ILogger<EpisodeRepository> logger
        )
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }
        public async Task<Episode?> CreateEpisode( //Task<ResultResponse<Episode>>
            Episode episode,
            IFormFile thumbnail,
            IFormFile file
        )
        {
            var series = await _context.Series
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == episode.SeriesId);

            if (series == null) {
                //  _logger.LogInformation("Found series: {Title}", series);
                throw new KeyNotFoundException($"Series with ID {episode.SeriesId} not found");
                //  return ResultResponse<Episode>.Fail(new NotFoundError { Description = "Series not found" });
            }

            if(series.Episodes.Any(a => 
                a.Season == episode.Season 
                && a.EpisodeNumber == episode.EpisodeNumber
            )){
                throw new InvalidOperationException($"Episode already exists in season {episode.Season} with number {episode.EpisodeNumber}");
                //  return ResultResponse<Episode>.Fail(new ConflictError { Description = "Episode already exists in this season and number" });
            }

            var safeTitle = MyRegex()
                    .Replace(CustomFunction
                    .SanitizeFolderName(series.Title)
                    .Trim().ToLower().Replace(" ", "_"), "");

            if (thumbnail != null && thumbnail.Length > 0)
            {
                var thumbFolder = $"uploads/series/{safeTitle}/episode";
                episode.Thumbnail = await _fileService.SaveFile(thumbnail, thumbFolder);
            }

    
            if (file != null && file.Length > 0)
            {
                var videoFolder = $"uploads/series/{safeTitle}/video";
                var videoPath = await _fileService.SaveFile(file, videoFolder);

                var video = new Video
                {
                    VideoUrl = videoPath,
                    Duration = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ViewCount = 0,
                    Episode = episode 
                };

                _context.Videos.Add(video);
            }

            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            return episode; //ResultResponse<Episode>.Success(episode);
        }


        public async Task<Episode?> DeleteEpisode(int id)
        {
            var filePaths = new List<string>();

            var episode = await _context.Episodes
                .Include(e => e.Video)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (episode == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(episode.Thumbnail))
            {
                filePaths.Add(episode.Thumbnail);
            }

            if (episode.Video != null && !string.IsNullOrWhiteSpace(episode.Video.VideoUrl))
            {
                filePaths.Add(episode.Video.VideoUrl);
                _context.Videos.Remove(episode.Video);
            }

            _fileService.DeleteFiles(filePaths);
            _context.Episodes.Remove(episode);
            await _context.SaveChangesAsync();

            return episode;
        }


        public async Task<List<Episode>> GetAllEpisodes(EpisodeQueryObject queryObject)
        {
            // throw new NotImplementedException();

            // return await _context.Episodes
            //     .Include(e => e.Series)
            //     .Where(e => e.Series.Title.ToLower() == queryObject.SeriesTitle.ToLower())
            //     .ToListAsync();

            var episodes = _context.Episodes
                .Include(e => e.Series)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.SeriesTitle))
            {
                episodes = episodes.Where(e =>
                    e.Series.Title.ToLower() == queryObject.SeriesTitle.ToLower());
            }

             var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await episodes.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Episode?> GetEpisode(int id)
        {
            return await _context
                .Episodes
                .Include(s => s.Video)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<Series?> UpdateEpisode(int id, CreateUpdateEpisodeDto dto, IFormFile? thumbnail = null)
        {
            throw new NotImplementedException();
        }

        [GeneratedRegex(@"[^a-z0-9_]")]
        private static partial Regex MyRegex();
    }
}