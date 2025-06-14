using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Video;
using application.IRepository;
using application.Mappers;
using domain.Entities;
using infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/video")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IVideoRepository _videoRepo;
        public VideoController(
            ApplicationDbContext context,
            IVideoRepository videoRepository
        )
        {
            _context = context;
            _videoRepo = videoRepository;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVideo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var video = await _videoRepo.GetVideo(id);

            if (video == null)
            {
                return NotFound();
            }

            return Ok(video.ToVideoDto());

        }

        [HttpPost]
        public async Task<IActionResult> CreateVideo(
            [FromForm] CreateUpdateVideoDto dto,
            [FromForm] IFormFile file
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var video = new Video
            {
                VideoUrl = dto.VideoUrl,
                // Duration = dto.Duration,
                CreatedAt = DateTime.UtcNow,
                // UpdatedAt = ,
                // ViewCount = ,
                EpisodeId = dto.EpisodeId,
            };

            var created = await _videoRepo.CreateVideo(
                video, file.OpenReadStream(), file.FileName
            );

            return CreatedAtAction(
                nameof(GetVideo),
                new { id = video.Id },
                video.ToVideoDto()
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var video = await _videoRepo.DeleteVideo(id);

            if (video == null)
            {
                return NotFound();
            }

            return NoContent();

        }

        [HttpGet]
        [Route("stream/{id}")]
        public async Task GetStream(int id)
        {
            var video = await _videoRepo.GetVideo(id);

            // Determine web root path based on environment
            string filepath = video?.VideoUrl ?? "";

            var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
               ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
               : Directory.GetCurrentDirectory();

            // Create full path by combining web root with the relative path
            var fullPath = Path.Combine(basePath, filepath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            // Ensure the file exists
            if (string.IsNullOrEmpty(filepath) || !System.IO.File.Exists(fullPath))
            {
                Response.StatusCode = 404;
                return;
            }

            string filename = Path.GetFileName(fullPath);
            Stream iStream = null;
            byte[] buffer = new byte[4096];
            int bytesRead;
            long totalBytes;

            try
            {
                // Open the file - using fullPath
                iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Total bytes to read
                totalBytes = iStream.Length;

                // Set the proper content type for MP4 files
                Response.ContentType = "video/mp4";
                Response.Headers["Accept-Ranges"] = "bytes";

                // Handle range requests for video seeking
                long startByte = 0;
                long endByte = totalBytes - 1;

                if (!string.IsNullOrEmpty(Request.Headers["Range"]))
                {
                    string rangeHeader = Request.Headers["Range"].ToString();
                    string[] rangeParts = rangeHeader.Replace("bytes=", "").Split('-');

                    if (rangeParts.Length > 0 && !string.IsNullOrEmpty(rangeParts[0]))
                    {
                        startByte = long.Parse(rangeParts[0]);
                    }

                    if (rangeParts.Length > 1 && !string.IsNullOrEmpty(rangeParts[1]))
                    {
                        endByte = long.Parse(rangeParts[1]);
                    }

                    // Seek to the requested position
                    iStream.Seek(startByte, SeekOrigin.Begin);

                    // Set partial content response
                    Response.StatusCode = 206;
                    Response.Headers["Content-Range"] = $"bytes {startByte}-{endByte}/{totalBytes}";
                    Response.Headers["Content-Length"] = (endByte - startByte + 1).ToString();
                }
                else
                {
                    // Full content response
                    Response.Headers["Content-Length"] = totalBytes.ToString();
                }

                long bytesToRead = endByte - startByte + 1;

                // Stream the file
                while (bytesToRead > 0)
                {
                    // Check if the client is still connected
                    if (Request.HttpContext.RequestAborted.IsCancellationRequested)
                        break;

                    // Read data into buffer
                    int readSize = (int)Math.Min(buffer.Length, bytesToRead);
                    bytesRead = await iStream.ReadAsync(buffer, 0, readSize);

                    if (bytesRead == 0)
                        break; // End of file

                    // Write to output stream
                    await Response.Body.WriteAsync(buffer, 0, bytesRead);
                    await Response.Body.FlushAsync();

                    // Update bytes remaining
                    bytesToRead -= bytesRead;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error streaming video: {ex.Message}");
                Response.StatusCode = 500;
            }
            finally
            {
                // Close the file
                iStream?.Close();
            }
        }

    }
}