using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.User;
using MediatR;

namespace application.Features.UserFeature.Register
{
    public sealed record class RegisterUserRequest(
        RegisterDto RegisterDto
    ) : IRequest<ResultResponse<RegisterUserResponse>>;
}