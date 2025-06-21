using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.User;
using AutoMapper;
using domain.Entities;

namespace application.Features.UserFeature.Login
{
    public class LoginUserMapper : Profile
    {
        public LoginUserMapper()
        {
            CreateMap<User, NewUserDto>();
        }
    }
}