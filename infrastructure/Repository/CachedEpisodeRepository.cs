using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using application.IRepository;
using domain.Entities;
using application.Common;
using application.Dtos.Episode;

namespace infrastructure.Repository
{
    public class CachedEpisodeRepository : IEpisodeRepository
    {
        private readonly EpisodeRepository _decorated;
        private readonly IDistributedCache _distributedCache;

        public CachedEpisodeRepository(
            EpisodeRepository decorated,
            IDistributedCache distributedCache
        )
        {
            _decorated = decorated;
            _distributedCache = distributedCache;
        }

        public Task<Episode?> CreateEpisode(
            Episode episode,
            Stream thumbnail,
            Stream file,
            string fileName
        )
        {
            return _decorated.CreateEpisode(
                episode,
                thumbnail,
                file,
                fileName
            );
        }

        public Task<Episode?> DeleteEpisode(int id)
        {
            return _decorated.DeleteEpisode(id);
        }

        public async Task<List<Episode>?> GetAllEpisodes(
            EpisodeQueryObject queryObject,
            CancellationToken cancellationToken = default
        )
        {
            string key = $"episodes-{queryObject.SeriesTitle}";

            string? cachedEpisodes = await _distributedCache.GetStringAsync(
                key,
                cancellationToken
            );

            List<Episode>? episodes;

            if (string.IsNullOrEmpty(cachedEpisodes))
            {

                episodes = await _decorated.GetAllEpisodes(
                    queryObject,
                    cancellationToken
                );

                if (episodes is null)
                {
                    return episodes;
                }

                await _distributedCache.SetStringAsync(
                    key,
                    JsonSerializer.Serialize(episodes),
                    cancellationToken
                );

                return episodes;

            }

            episodes = JsonSerializer.Deserialize<List<Episode>>(cachedEpisodes);

            return episodes;

        }

        public Task<Episode?> GetEpisode(int id)
        {
            return _decorated.GetEpisode(id);
        }

        public Task<Series?> UpdateEpisode(
            int id,
            CreateUpdateEpisodeDto dto,
            Stream? thumbnail = null
        )
        {
            return _decorated.UpdateEpisode(
                id,
                dto,
                thumbnail
            );
        }
    }
}