using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.IRepository
{
    public interface ITokenService
    {
        string CreateToken(User user);
        Task BlacklistTokenAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task CleanupExpiredTokensAsync();
    }
}