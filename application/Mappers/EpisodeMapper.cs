using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Episode;
using domain.Entities;

namespace application.Mappers
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
                Thumbnail = episode.Thumbnail,
                CreatedAt = episode.CreatedAt,
                Season = episode.Season,
                EpisodeNumber = episode.EpisodeNumber,
                SeriesId = episode.SeriesId,
            };
        }

        public static EpisodeVideoDto ToEpisodeVideoDto(this Episode episode)
        {
            return new EpisodeVideoDto
            {
                Id = episode.Id,
                Title = episode.Title,
                Description = episode.Description,
                Thumbnail = episode.Thumbnail,
                CreatedAt = episode.CreatedAt,
                Season = episode.Season,
                EpisodeNumber = episode.EpisodeNumber,
                SeriesId = episode.SeriesId,
                Video = episode?.Video?.ToVideoDto(),
            };
        }

    }
}