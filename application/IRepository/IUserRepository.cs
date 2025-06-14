using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using domain.Entities;

namespace application.IRepository
{
    public interface IUserRepository
    {
        Task<(User user, string token)> LoginWithGoogle(
            ClaimsPrincipal? claimsPrincipal
        );
    }
}