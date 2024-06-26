using Application.DTO.Workspace;

namespace Application.UseCases.Queries.User
{
    public interface IGetWorkspaceQuery : IQuery<WorkspaceDto, int>
    {
    }
}
