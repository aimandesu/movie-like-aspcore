using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Video;
using domain.Entities;

namespace application.Mappers
{
    public static class VideoMapper
    {
        public static VideoDto ToVideoDto(this Video video)
        {
            return new VideoDto
            {
                Id = video.Id,
                VideoUrl = video.VideoUrl,
                Duration = video.Duration,
                CreatedAt = video.CreatedAt,
                UpdatedAt = video.UpdatedAt,
                ViewCount = video.ViewCount,
                EpisodeId = video.EpisodeId,
            };
        }
    }
}