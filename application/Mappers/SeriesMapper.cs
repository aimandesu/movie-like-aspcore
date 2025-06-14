using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Series;
using domain.Entities;

namespace application.Mappers
{
    public static class SeriesMapper
    {
        public static SeriesDto ToSeriesDto(this Series series)
        {
            return new SeriesDto
            {
                Id = series.Id,
                Slug = series.Slug,
                Title = series.Title,
                Description = series.Description,
                Thumbnail = series.Thumbnail,
                CreatedAt = series.CreatedAt,
                SeriesFormat = series.SeriesFormat,
                SeriesCategories = [.. series.SeriesCategories.Select(sc => sc.ToSeriesCategoryDto())],
                TagCategories = [.. series.TagCategories.Select(sc => sc.ToTagCategoryDto())],
            };
        }

        public static SeriesEpisodeDto ToSeriesEpisodeDto(this Series series)
        {
            return new SeriesEpisodeDto
            {
                Id = series.Id,
                Slug = series.Slug,
                Title = series.Title,
                Description = series.Description,
                Thumbnail = series.Thumbnail,
                CreatedAt = series.CreatedAt,
                SeriesFormat = series.SeriesFormat,
                SeriesCategories = [.. series.SeriesCategories.Select(sc => sc.ToSeriesCategoryDto())],
                TagCategories = [.. series.TagCategories.Select(sc => sc.ToTagCategoryDto())],
                // Episodes = [.. series.Episodes.Select(sc => sc.ToEpisodeDto())]
                Seasons = series.Episodes
                    .GroupBy(e => e.Season) // Group by Season (null = movies)
                    .OrderBy(g => g.Key ?? int.MaxValue) // Seasons first, then movies
                    .Select(g => new SeasonDto
                    {
                        SeasonNumber = g.Key, // null for movies
                        Episodes = g
                            .OrderBy(e => e.EpisodeNumber ?? int.MaxValue)
                            .Select(e => e.ToEpisodeDto())
                            .ToList()
                    })
                    .ToList(),
                Comments = [.. series.Comments.Select(c => c.ToCommentDto())],
            };
        }

    }
}