using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/seriestype")]
    [ApiController]
    public class SeriesTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISeriesTypeRepository _seriesTypeRepo;

        public SeriesTypeController(
            ApplicationDbContext context,
            ISeriesTypeRepository seriesTypeRepository
        )
        {
            _context = context;
            _seriesTypeRepo = seriesTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var seriesTypes = await _seriesTypeRepo.GetAllAsync();
            
            return Ok(seriesTypes);

        }

    }
}