using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Category;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(
            ApplicationDbContext context, 
            ICategoryRepository categoryRepository
        )
        {
            _context = context;
            _categoryRepo = categoryRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var categories = await _categoryRepo.GetAllCategories();

            return Ok(categories);

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var category = await _categoryRepo.GetCategory(id);

            if(category == null)
            {
                return NotFound();
            }

            return Ok(category);

        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
            };

            await _categoryRepo.CreateCategory(category);

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryRepo.DeleteCategory(id);

            if(category == null)
            {
                return NotFound();
            }

            return NoContent();
            
        }

    }
}