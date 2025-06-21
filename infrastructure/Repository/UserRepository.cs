using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.User;
using application.IRepository;
using domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository
{
    public partial class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public UserRepository(
            UserManager<User> userManager,
            ITokenService tokenService,
            SignInManager<User> signInManager
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        public async Task<(User user, string token)> LoginWithGoogle(ClaimsPrincipal? claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new Exception("Claims principal is null");
            }

            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                throw new Exception("Email principal is null");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var givenName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);  //ClaimTypes.GivenName gives me my account name, for example 知らない人

                var asciiOnly = MyRegex();
                var userName = !string.IsNullOrWhiteSpace(givenName) && asciiOnly.IsMatch(givenName)
                    ? givenName
                    : email.Split('@')[0];


                //create user because user not existed in db
                var newUser = new User
                {
                    UserName = userName,
                    Email = email,

                };

                var result = await _userManager.CreateAsync(newUser);

                if (!result.Succeeded)
                {
                    throw new Exception($"Unable to create user {string.Join(", ", result.Errors.Select((x) => x.Description))}");
                }

                user = newUser;

            }

            var loginProviderKey = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier); // Usually Google sub
            var info = new UserLoginInfo("Google", loginProviderKey!, "Google");

            var existingLogins = await _userManager.GetLoginsAsync(user);
            if (!existingLogins.Any(l => l.LoginProvider == "Google"))
            {
                var loginResult = await _userManager.AddLoginAsync(user, info);

                if (!loginResult.Succeeded)
                {
                    throw new Exception($"Unable to login user {string.Join(", ", loginResult.Errors.Select((x) => x.Description))}");
                }
            }

            var token = _tokenService.CreateToken(user);

            return (user, token);

        }

        [GeneratedRegex("^[a-zA-Z0-9]+$")]
        private static partial Regex MyRegex();

        public async Task<ResultResponse<User>> RegisterUser(
            RegisterDto registerDto
        )
        {

            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };

            var createdUser = await _userManager.CreateAsync(
                   user,
                   registerDto?.Password ?? ""
               );

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager
                    .AddToRoleAsync(user, "User");

                if (roleResult.Succeeded)
                {
                    return ResultResponse<User>.Success(user);
                }
                else
                {
                    return ResultResponse<User>.Fail(
                        new NotFoundError
                        {
                            Description = string.Join(", ", createdUser.Errors.Select(e => e.Description))
                        }
                    );
                }
            }
            else
            {
                return ResultResponse<User>.Fail(new NotFoundError
                {
                    Description = string.Join(", ", createdUser.Errors.Select(e => e.Description))
                });
            }
        }

        public async Task<ResultResponse<User>> LoginWithEmail(
            LoginDto loginDto
        )
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(
                    x => x.UserName == loginDto.Username
                );

            if (user == null)
            {
                return ResultResponse<User>.Fail(new NotFoundError
                {
                    Description = "User not found"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                loginDto.Password,
                false
            );

            if (!result.Succeeded)
            {
                return ResultResponse<User>.Fail(new NotFoundError
                {
                    Description = "Username not found and/or wrong password"
                });
            }

            return ResultResponse<User>.Success(user);

        }
    }
}