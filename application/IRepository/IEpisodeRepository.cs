using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Episode;
using domain.Entities;

namespace application.IRepository
{
    public interface IEpisodeRepository
    {
        Task<List<Episode>?> GetAllEpisodes(
            EpisodeQueryObject queryObject,
            CancellationToken cancellationToken
        );
        Task<Episode?> GetEpisode(int id);
        Task<Episode?> CreateEpisode(
            Episode episode,
            Stream thumbnail,
            Stream file,
            string fileName
        );
        Task<Episode?> DeleteEpisode(int id);
        Task<Series?> UpdateEpisode(
            int id,
            CreateUpdateEpisodeDto dto,
            Stream? thumbnail = null
        );
    }
}