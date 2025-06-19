using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Category;
using application.Features.CategoryFeature.Create;
using application.Features.CategoryFeature.Delete;
using application.IRepository;
using domain.Entities;
using infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMediator _mediator;

        public CategoryController(
            ApplicationDbContext context,
            ICategoryRepository categoryRepository,
            IMediator mediator
        )
        {
            _context = context;
            _categoryRepo = categoryRepository;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categories = await _categoryRepo.GetAllCategories();

            return Ok(categories);

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryRepo.GetCategory(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);

        }

        [HttpPost]
        public async Task<ActionResult<CreateCategoryResponse>> CreateCategory(
            [FromForm] CreateCategoryDto dto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(
                new CreateCategoryRequest(dto)
            );

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = result?.Category?.Id }, result?.Category
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> Delete(
            [FromRoute] int id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _mediator.Send(
               new DeleteCategoryRequest(id)
           );

            if (category == null)
            {
                return NotFound();
            }

            return NoContent();

        }

    }
}