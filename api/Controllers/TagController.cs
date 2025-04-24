using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Tag;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    [Route("api/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITagRepository _tagRepo;
        public TagController(
            ApplicationDbContext context, 
            ITagRepository tagRepository
        )
        {
            _context = context;
            _tagRepo = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var tags = await _tagRepo.GetAllTags();

            return Ok(tags);

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTag([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var tag = await _tagRepo.GetTag(id);

            if(tag == null)
            {
                return NotFound();
            }

            return Ok(tag);

        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromForm] CreateTagDto dto)
        {
            var tag = new Tag
            {
                Name = dto.Name,
            };

            await _tagRepo.CreateTag(tag);

            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var tag = await _tagRepo.DeleteTag(id);

            if(tag == null)
            {
                return NotFound();
            }

            return NoContent();
            
        }

    }
}