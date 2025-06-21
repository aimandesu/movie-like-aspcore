using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.User;
using application.IRepository;
using AutoMapper;
using MediatR;

namespace application.Features.UserFeature.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserRequest, ResultResponse<LoginUserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LoginUserHandler(
            IUserRepository userRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResultResponse<LoginUserResponse>> Handle(
            LoginUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await _userRepository.LoginWithEmail(request.LoginDto);

            if (!result.IsSuccess)
            {
                return ResultResponse<LoginUserResponse>.Fail(result.Error);
            }

            var newUserDto = _mapper.Map<NewUserDto>(result.Data);

            return ResultResponse<LoginUserResponse>.Success(
                new LoginUserResponse
                { NewUserDto = newUserDto }
            );

        }
    }
}