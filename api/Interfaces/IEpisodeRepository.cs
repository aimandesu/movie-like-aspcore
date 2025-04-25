using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Episode;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IEpisodeRepository
    {
        Task<List<Episode>> GetAllEpisodes(EpisodeQueryObject queryObject);
        Task<Episode?> GetEpisode(int id);
        Task<Episode?> CreateEpisode(
            Episode episode, 
            IFormFile thumbnail,
            IFormFile file
        );
        Task<Episode?> DeleteEpisode(int id);
        Task<Series?> UpdateEpisode(
            int id, 
            CreateUpdateEpisodeDto dto, 
            IFormFile? thumbnail = null
        );
    }
}