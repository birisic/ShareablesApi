using Application;
using Application.DTO.Workspace;
using Domain;

namespace Implementation
{
    public class Actor : IApplicationActor
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public IEnumerable<WorkspaceUseCases> WorkspacesUseCases { get; set; }
    }

    public class UnauthorizedActor : IApplicationActor
    {
        public int Id => 0;
        public string Username => "unauthorized";
        public IEnumerable<WorkspaceUseCases> WorkspacesUseCases => new List<WorkspaceUseCases> {
            new WorkspaceUseCases { 
                WorkspaceId = 0, 
                UseCaseIds = [(int)UseCasesEnum.UserRegistration]
            }
        };
    }
}
