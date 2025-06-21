using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using application.Dtos.User;
using MediatR;

namespace application.Features.UserFeature.Login
{
    public sealed record class LoginUserRequest(
        LoginDto LoginDto
    ) : IRequest<ResultResponse<LoginUserResponse>>;
}