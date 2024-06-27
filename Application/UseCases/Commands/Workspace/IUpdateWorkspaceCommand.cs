using Application.DTO.Workspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Commands.Workspace
{
    public interface IUpdateWorkspaceCommand : ICommand<WorkspaceDto>
    {
    }
}
