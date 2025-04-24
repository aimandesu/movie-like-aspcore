using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Series;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/series")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISeriesRepository _seriesRepo;
        public SeriesController(
            ApplicationDbContext context,
            ISeriesRepository seriesRepository
        )
        {
            _context = context;
            _seriesRepo = seriesRepository;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSeries([FromQuery] SeriesQueryObject queryObject)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var series =  await _seriesRepo.GetAllSeries(queryObject);

            var seriesDto = series.Select(s => s.ToSeriesDto());

            return Ok(seriesDto);

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSeries([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var series = await _seriesRepo.GetSeries(id);

            if(series == null)
            {
                return NotFound();
            }

            return Ok(series);

        }

        [HttpPost]
        public async Task<IActionResult> CreateSeries([FromForm] CreateUpdateSeriesDto dto, [FromForm] IFormFile thumbnail)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var series = new Series
            {
                Title = dto.Title,
                Description = dto.Description,
                Thumbnail = dto.Thumbnail,
                SeriesFormat = SeriesFormat.Single,
                CreatedAt = DateTime.UtcNow,
                SeriesCategories = [.. dto.CategoryIds.Select(catId => new SeriesCategory
                {
                    CategoryId = catId
                })]
            };

            var created = await _seriesRepo.CreateSeries(series, thumbnail);
            
            return CreatedAtAction(nameof(GetSeries), new { id = series.Id }, series);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromForm] CreateUpdateSeriesDto dto,
            [FromForm] IFormFile? thumbnail = null
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var series = await _seriesRepo.UpdateSeries(id, dto, thumbnail);

            if (series == null)
                return NotFound();

            return Ok(series);
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var series = await _seriesRepo.DeleteSeries(id);

            if(series == null)
            {
                return NotFound();
            }

            return NoContent();
            
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
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", series.Thumbnail.TrimStart('/'));
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