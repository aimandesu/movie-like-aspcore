using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Episode;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    [Route("api/episode")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEpisodeRepository _episodeRepo;
        public EpisodeController(
            ApplicationDbContext context,
            IEpisodeRepository episodeRepository
        )
        {
            _context = context;
            _episodeRepo = episodeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEpisodes([FromQuery] EpisodeQueryObject queryObject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (queryObject == null)
                return BadRequest("Series title is required.");

            var episodes = await _episodeRepo.GetAllEpisodes(queryObject);

            return Ok(episodes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEpisode([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var series = await _episodeRepo.GetEpisode(id);

            if(series == null)
            {
                return NotFound();
            }

            return Ok(series);

        }

        [HttpPost]
        public async Task<IActionResult> CreateEpisode(
            [FromForm] CreateUpdateEpisodeDto dto, 
            [FromForm] IFormFile thumbnail,
            [FromForm] IFormFile file
        )
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var episode = new Episode
            {
                Title = dto.Title,
                Description = dto.Description,
                Thumbnail = dto.Thumbnail,
                CreatedAt = DateTime.UtcNow,
                SeriesId = dto.SeriesId,
            };

            var created = await _episodeRepo.CreateEpisode(episode, thumbnail, file);
            
            return CreatedAtAction(nameof(GetEpisode), new { id = episode.Id }, episode.ToEpisodeDto());
        }

    }
}