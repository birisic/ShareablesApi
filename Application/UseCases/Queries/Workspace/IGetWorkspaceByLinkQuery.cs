using Application.DTO.Workspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Queries.Workspace
{
    public interface IGetWorkspaceByLinkQuery : IQuery<WorkspaceDto, string>
    {
    }
}
