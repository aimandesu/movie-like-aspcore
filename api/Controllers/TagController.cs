using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Tag;
using application.Features.TagFeature.Create;
using application.Features.TagFeature.Delete;
using application.IRepository;
using domain.Entities;
using infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    [Route("api/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITagRepository _tagRepo;
        private readonly IMediator _mediator;

        public TagController(
            ApplicationDbContext context,
            ITagRepository tagRepository,
            IMediator mediator
        )
        {
            _context = context;
            _tagRepo = tagRepository;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tags = await _tagRepo.GetAllTags();

            return Ok(tags);

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tag = await _tagRepo.GetTag(id);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);

        }

        [HttpPost]
        public async Task<ActionResult<CreateTagResponse>> CreateTag(
            [FromForm] CreateTagDto dto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(
                new CreateTagRequest(dto)
            );

            return CreatedAtAction(
                nameof(GetTag),
                new { id = result?.Tag?.Id },
                result?.Tag
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<Tag>> Delete(
            [FromRoute] int id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tag = await _mediator.Send(
               new DeleteTagRequest(id)
           );

            if (tag == null)
            {
                return NotFound();
            }

            return NoContent();

        }

    }
}