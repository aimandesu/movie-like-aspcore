using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.Series;
using application.Features.SeriesFeature.Create;
using application.Features.SeriesFeature.Delete;
using application.Features.SeriesFeature.Update;
using application.IRepository;
using application.Mappers;
using domain.Entities;
using infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/series")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context; //this should be removed
        private readonly ISeriesRepository _seriesRepo; // this should be removed

        private readonly IMediator _mediator;

        public SeriesController(
            ApplicationDbContext context,
            ISeriesRepository seriesRepository,
            IMediator mediator
        )
        {
            _context = context;
            _seriesRepo = seriesRepository;
            _mediator = mediator;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSeries(
            [FromQuery] SeriesQueryObject queryObject,
            [FromQuery] PaginationQueryObject pagination
            )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var series = await _seriesRepo.GetAllSeries(queryObject, pagination);

            var seriesDto = series.Select(s => s.ToSeriesDto());

            return Ok(seriesDto);

        }

        [HttpGet("{slug}", Name = "GetSeriesBySlug")]
        public async Task<IActionResult> GetSeries([FromRoute] string slug)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var series = await _seriesRepo.GetSeries(slug);

            if (series == null)
            {
                return NotFound();
            }

            return Ok(series.ToSeriesEpisodeDto());

        }

        [HttpPost]
        public async Task<ActionResult<CreateSeriesResponse>> CreateSeries(
            [FromForm] CreateUpdateSeriesDto dto,
            [FromForm] IFormFile thumbnail,
            CancellationToken cancellationToken
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateSeriesRequest(
                dto,
                new FileUploadDto(
                    thumbnail.OpenReadStream(),
                    thumbnail.FileName,
                    thumbnail.ContentType
                )
            );

            var result = await _mediator.Send(
                command,
                cancellationToken
            );

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UpdateSeriesResponse>> Update(
            [FromRoute] int id,
            [FromForm] CreateUpdateSeriesDto dto,
            CancellationToken cancellationToken,
            [FromForm] IFormFile? thumbnail = null
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateSeriesRequest(
             id,
             dto,
             thumbnail != null ?
              new FileUploadDto(
                    thumbnail.OpenReadStream(),
                    thumbnail.FileName,
                    thumbnail.ContentType
                ) : null
            );

            var result = await _mediator.Send(
                command,
                cancellationToken
            );

            return Ok(result);
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<DeleteSeriesResponse>> Delete(
            [FromRoute] int id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(
             new DeleteSeriesRequest(id)
           );

            return Ok(result);

        }

        [HttpDelete("delete_all_series")]
        public async Task<IActionResult> DeleteAllSeries()
        {
            var seriesList = await _context.Series.ToListAsync();

            if (seriesList.Count == 0)
                return NotFound("No series found to delete.");

            foreach (var series in seriesList)
            {
                if (!string.IsNullOrEmpty(series.Thumbnail))
                {
                    var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                        ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                        : Directory.GetCurrentDirectory();

                    var filePath = Path.Combine(basePath, series.Thumbnail.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            _context.Series.RemoveRange(seriesList);
            await _context.SaveChangesAsync();

            return Ok("All series and their images have been deleted.");
        }
    }
}