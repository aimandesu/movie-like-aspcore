using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.User;
using AutoMapper;
using domain.Entities;

namespace application.Features.UserFeature.Register
{
    public class RegisterUserMapper : Profile
    {
        public RegisterUserMapper()
        {
            CreateMap<User, NewUserDto>();
        }
    }
}