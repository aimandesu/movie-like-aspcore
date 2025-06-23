using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Episode;
using application.Features.EpisodeFeature.Create;
using application.Features.EpisodeFeature.Delete;
using application.IRepository;
using application.Mappers;
using domain.Entities;
using infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    [Route("api/episode")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEpisodeRepository _episodeRepo;
        private readonly IMediator _mediator;

        public EpisodeController(
            ApplicationDbContext context,
            IEpisodeRepository episodeRepository,
            IMediator mediator
        )
        {
            _context = context;
            _episodeRepo = episodeRepository;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEpisodes(
            [FromQuery] EpisodeQueryObject queryObject,
            CancellationToken cancellationToken
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(queryObject.SeriesTitle))
                return BadRequest("Series title is required.");

            var episodes = await _episodeRepo.GetAllEpisodes(
                queryObject,
                cancellationToken
            );

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
        public async Task<ActionResult<CreateEpisodeResponse>> CreateEpisode(
            [FromForm] CreateUpdateEpisodeDto dto,
            [FromForm] IFormFile thumbnail,
            [FromForm] IFormFile file,
            CancellationToken cancellationToken
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateEpisodeRequest(
                CreateUpdateEpisodeDto: dto,
                Thumbnail: new FileUploadDto(
                    thumbnail.OpenReadStream(),
                    thumbnail.FileName,
                    thumbnail.ContentType
                ),
                File: new FileUploadDto(
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType
                )
            );

            var result = await _mediator.Send(
                command,
                cancellationToken
            );

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

            return CreatedAtAction(
                nameof(GetEpisode),
                new { id = result?.EpisodeDto?.Id },
                result?.EpisodeDto
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<DeleteEpisodeResponse>> Delete(
            [FromRoute] int id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var episode = await _mediator.Send(
                new DeleteEpisodeRequest(id)
            );

            if (episode == null)
            {
                return NotFound();
            }

            // return NoContent();
            return Ok(episode);

        }

    }
}