using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.User;
using domain.Entities;

namespace application.IRepository
{
    public interface IUserRepository
    {
        Task<ResultResponse<User>> RegisterUser(RegisterDto registerDto);
        Task<ResultResponse<User>> LoginWithEmail(LoginDto loginDto);
        Task<(User user, string token)> LoginWithGoogle(
            ClaimsPrincipal? claimsPrincipal
        );

    }
}