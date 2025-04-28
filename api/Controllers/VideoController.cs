using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Video;
using api.Interfaces;
using api.Mappers;
using api.Models;
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
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var video = await _videoRepo.GetVideo(id);

            if(video == null)
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
            if(!ModelState.IsValid)
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

            var created = await _videoRepo.CreateVideo(video, file);
            
            return CreatedAtAction(nameof(GetVideo), new { id = video.Id }, video.ToVideoDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var video = await _videoRepo.DeleteVideo(id);

            if(video == null)
            {
                return NotFound();
            }

            return NoContent();
            
        }

        [HttpGet]
        [Route("stream/{id}")]
        public async Task GetStream(int id)
        {
            await _videoRepo.StreamVideo(id, Response.Body, Request, Response);
        }

    }
}