using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using application.Common;
using application.Dtos.Comment;
using application.IRepository;
using application.Mappers;
using domain.Entities;
using infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICommentRepository _commentRepo;
        public CommentController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ICommentRepository commentRepository
        )
        {
            _context = context;
            _userManager = userManager;
            _commentRepo = commentRepository;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetComment(
            [FromRoute] int id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepo.GetComment(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment(
            [FromQuery] PaginationQueryObject pagination,
            [FromQuery] CommentQueryObject commentQuery,
            int episodeId
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comments = await _commentRepo.GetAllComments(
                pagination,
                commentQuery,
                episodeId
            );

            return Ok(comments.Select(e => e.ToCommentDto()));

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(
            [FromForm] CreateUpdateCommentDto dto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);

            var comment = new Comment
            {
                EpisodeId = dto.EpisodeId,
                Discussion = dto.Discussion,
                UserId = user?.Id ?? "",
                CreatedAt = DateTime.UtcNow,
            };

            await _commentRepo.AddComment(comment);

            return CreatedAtAction(
                nameof(GetComment),
                new { id = comment.Id },
                comment.ToCommentDto()
            );

        }

        [HttpDelete]
        [Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            var username = User.GetUsername();

            var comment = await _commentRepo.DeleteComment(id, username);

            if (comment == null)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}