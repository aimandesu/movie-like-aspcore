using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.User;
using application.Features.UserFeature.Login;
using application.Features.UserFeature.Register;
using application.IRepository;
using domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepo;
        private readonly IMediator _mediator;

        public UserController(
            UserManager<User> userManager,
            ITokenService tokenService,
            SignInManager<User> signInManager,
            IUserRepository userRepository,
            IMediator mediator
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userRepo = userRepository;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponse>> Register(
            [FromForm] RegisterDto registerDto,
            CancellationToken cancellationToken
        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var command = new RegisterUserRequest(
                    registerDto
                );

                var result = await _mediator.Send(
                    command,
                    cancellationToken
                );

                if (result.IsSuccess)
                {
                    //append here to token 
                    var token = _tokenService.CreateToken(
                        new User
                        {
                            Email = registerDto.Email,
                            UserName = registerDto.Username,
                            Id = result?.Data?.NewUserDto?.Id ?? ""
                        }
                    );

                    //append to jwt
                    HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(7)
                    });

                    return Ok(result?.Data);
                }
                else
                {
                    return StatusCode(500, result.Error);
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginUserResponse>> Login(
            [FromForm] LoginDto loginDto,
            CancellationToken cancellationToken
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new LoginUserRequest(
                loginDto
            );

            var result = await _mediator.Send(
                command,
                cancellationToken
            );

            if (result.IsSuccess)
            {
                var token = _tokenService.CreateToken(
                        new User
                        {
                            Email = result?.Data?.NewUserDto?.Email,
                            UserName = result?.Data?.NewUserDto?.UserName,
                            Id = result?.Data?.NewUserDto?.Id ?? ""
                        }
                    );

                //append to jwt
                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(result?.Data);
            }
            else
            {
                return StatusCode(500, result.Error);
            }


        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "User logged out successfully" });
        }

        [HttpGet("account/login/google")]
        public async Task<IActionResult> GoogleLogin(
            [FromQuery] string returnUrl,
            LinkGenerator linkGenerator,
            SignInManager<User> signManager
        )
        {
            var path = linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback");
            var properties = signManager.ConfigureExternalAuthenticationProperties(
                "Google", $"{path}?returnUrl={returnUrl}"
            );

            return Challenge(properties, "Google");
        }


        [HttpGet("account/login/google/callback", Name = "GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallback(
            [FromQuery] string returnUrl
        )
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var (user, token) = await _userRepo.LoginWithGoogle(result.Principal);

            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Redirect(returnUrl);
        }


    }
}