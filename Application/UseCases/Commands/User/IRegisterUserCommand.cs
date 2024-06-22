using System;
using System.Collections.Generic;
using System.Text;
using Application.DTO.User;

namespace Application.UseCases.Commands.User
{
    public interface IRegisterUserCommand : ICommand<UserAuthRequestDto>
    {
    }
}
