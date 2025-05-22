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
        public async Task<IActionResult> GetAllEpisodes([FromQuery] EpisodeQueryObject queryObject, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(queryObject.SeriesTitle))
                return BadRequest("Series title is required.");

            var episodes = await _episodeRepo.GetAllEpisodes(queryObject, cancellationToken);

            return Ok(episodes.Select(s => s.ToEpisodeDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEpisode([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var episode = await _episodeRepo.GetEpisode(id);

            if (episode == null)
            {
                return NotFound();
            }

            return Ok(episode.ToEpisodeVideoDto());

        }

        [HttpPost]
        public async Task<IActionResult> CreateEpisode(
            [FromForm] CreateUpdateEpisodeDto dto,
            [FromForm] IFormFile thumbnail,
            [FromForm] IFormFile file
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var episode = new Episode
            {
                Title = dto.Title,
                Description = dto.Description,
                Thumbnail = dto.Thumbnail,
                CreatedAt = DateTime.UtcNow,
                SeriesId = dto.SeriesId,
                Season = dto.Season,
                EpisodeNumber = dto.EpisodeNumber,
            };

            var result = await _episodeRepo.CreateEpisode(episode, thumbnail, file);

            // if (!result.IsSuccess)
            // {

            //     if (result.Error is NotFoundError)
            //     {
            //         return NotFound(new { error = result.Error.Description });
            //     }
            //     else if (result.Error is ConflictError)
            //     {
            //         return Conflict(new { error = result.Error.Description });
            //     }
            //     else
            //     {
            //         return BadRequest(new { error = result?.Error?.Description });
            //     }
            // }

            return CreatedAtAction(nameof(GetEpisode), new { id = episode.Id }, episode.ToEpisodeDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var series = await _episodeRepo.DeleteEpisode(id);

            if (series == null)
            {
                return NotFound();
            }

            return NoContent();

        }

    }
}