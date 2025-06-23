using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Episode;
using application.IRepository;
using domain.Entities;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace infrastructure.Repository
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
        public Episode CreateEpisode( //Task<ResultResponse<Episode>>
            Episode episode,
            Video video
        )
        {
            _context.Videos.Add(video);
            _context.Episodes.Add(episode);

            return episode; //ResultResponse<Episode>.Success(episode);
        }


        public Episode? DeleteEpisode(
            Episode episode
        )
        {

            if (episode.Video != null && !string.IsNullOrWhiteSpace(episode.Video.VideoUrl))
            {

                _context.Videos.Remove(episode.Video);
            }

            _context.Episodes.Remove(episode);

            return episode;
        }


        public async Task<List<Episode>?> GetAllEpisodes(
            EpisodeQueryObject queryObject,
            CancellationToken cancellationToken
        )
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

        public Task<Series?> UpdateEpisode(int id, CreateUpdateEpisodeDto dto, Stream? thumbnail = null)
        {
            throw new NotImplementedException();
        }

        [GeneratedRegex(@"[^a-z0-9_]")]
        private static partial Regex MyRegex();
    }
}