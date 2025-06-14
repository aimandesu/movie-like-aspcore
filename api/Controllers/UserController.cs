using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.User;
using application.IRepository;
using domain.Entities;
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

        public UserController(
            UserManager<User> userManager,
            ITokenService tokenService,
            SignInManager<User> signInManager,
            IUserRepository userRepository
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userRepo = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromForm] RegisterDto registerDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };

                var createdUser = await _userManager.CreateAsync(
                    user,
                    registerDto.Password
                );

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager
                        .AddToRoleAsync(user, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                Token = _tokenService.CreateToken(user),
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromForm] LoginDto loginDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users
                .FirstOrDefaultAsync(
                    x => x.UserName == loginDto.Username
                );

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                loginDto.Password,
                false
            );

            if (!result.Succeeded) return Unauthorized("Username not found and/or wrong password");

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
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