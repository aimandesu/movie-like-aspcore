using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace application.IRepository
{
    public interface IUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        public bool IsAuthenticated { get; }
    }
}