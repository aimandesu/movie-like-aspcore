using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Episode;
using api.Models;

namespace api.Mappers
{
    public static class EpisodeMapper
    {
        public static EpisodeDto ToEpisodeDto(this Episode episode)
        {
            return new EpisodeDto
            {
                Id = episode.Id,
                Title = episode.Title,
                Description = episode.Description,
                Thumbnail  = episode.Thumbnail,
                CreatedAt = episode.CreatedAt,
                Season = episode.Season,
                EpisodeNumber  = episode.EpisodeNumber,
                SeriesId = episode.SeriesId,
            };
        }
    }
}