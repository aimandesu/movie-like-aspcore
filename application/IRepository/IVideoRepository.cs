using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Video;
using domain.Entities;

namespace application.IRepository
{
    public interface IVideoRepository
    {
        Task<Video?> GetVideo(int id);
        Task<Video?> CreateVideo(
            Video video,
            Stream file,
            string fileName
        );
        Task<Video?> DeleteVideo(int id);
        Task<Video?> UpdateVideo(
            int id,
            CreateUpdateVideoDto dto,
            Stream? video
        );
        // Task StreamVideo(
        //     int id, 
        //     Stream outputStream, 
        //     HttpRequest request, 
        //     HttpResponse response
        // );
    }
}