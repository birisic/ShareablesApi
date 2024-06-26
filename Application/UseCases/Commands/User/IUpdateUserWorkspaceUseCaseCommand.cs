using Application.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Commands.User
{
    public interface IUpdateUserWorkspaceUseCaseCommand : ICommand<UserWorkspaceUseCaseDto>
    {
    }
}
