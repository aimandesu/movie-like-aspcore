using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Video;
using api.Models;

namespace api.Interfaces
{
    public interface IVideoRepository
    {
        Task<Video?> GetVideo(int id);
        Task<Video?> CreateVideo(
            Video video, 
            IFormFile file
        );
        Task<Video?> DeleteVideo(int id);
        Task<Video?> UpdateVideo(
            int id, 
            CreateUpdateVideoDto dto, 
            IFormFile? video
        );
        Task StreamVideo(
            int id, 
            Stream outputStream, 
            HttpRequest request, 
            HttpResponse response
        );
    }
}