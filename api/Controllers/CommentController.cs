using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using application.Common;
using application.Dtos.Comment;
using application.Features.CommentFeature.Create;
using application.Features.CommentFeature.Delete;
using application.IRepository;
using application.Mappers;
using domain.Entities;
using infrastructure.Data;
using MediatR;
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
        private readonly IMediator _mediator;
        public CommentController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ICommentRepository commentRepository,
            IMediator mediator
        )
        {
            _context = context;
            _userManager = userManager;
            _commentRepo = commentRepository;
            _mediator = mediator;
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<CreateCommentResponse>> AddComment(
            [FromForm] CreateUpdateCommentDto dto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(
                new CreateCommentRequest(dto)
            );

            return CreatedAtAction(
                nameof(GetComment),
                new { id = result?.Comment?.Id },
                result?.Comment?.ToCommentDto()
            );

        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("{id:int}")]
        public async Task<ActionResult<Comment?>> Delete(
            [FromRoute] int id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // var username = User.GetUsername();

            var result = await _mediator.Send(
                 new DeleteCommentRequest(id)
             );


            if (result == null)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}