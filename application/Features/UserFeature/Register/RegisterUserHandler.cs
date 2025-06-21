using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.User;
using application.IRepository;
using AutoMapper;
using domain.Entities;
using MediatR;

namespace application.Features.UserFeature.Register
{
    public class RegisterUserHandler : IRequestHandler<
    RegisterUserRequest, ResultResponse<RegisterUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public RegisterUserHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task<ResultResponse<RegisterUserResponse>> Handle(
            RegisterUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await _userRepository.RegisterUser(request.RegisterDto);

            if (!result.IsSuccess)
            {
                return ResultResponse<RegisterUserResponse>.Fail(result.Error);
            }

            // await _unitOfWork.SaveAsync(cancellationToken);

            var newUserDto = _mapper.Map<NewUserDto>(result.Data);

            return ResultResponse<RegisterUserResponse>.Success(
                new RegisterUserResponse
                { NewUserDto = newUserDto }
            );
        }
    }
}