using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.User;

namespace application.Features.UserFeature.Register
{
    public class RegisterUserResponse
    {
        public NewUserDto? NewUserDto { get; set; }
    }
}