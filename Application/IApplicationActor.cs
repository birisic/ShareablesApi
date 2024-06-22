using Application.DTO.Workspace;
using Domain;
using System.Collections.Generic;

namespace Application
{
    public interface IApplicationActor
    {
        int Id { get; }
        string Username { get; }
        IEnumerable<WorkspaceUseCases> WorkspacesUseCases { get; }
    }
}
