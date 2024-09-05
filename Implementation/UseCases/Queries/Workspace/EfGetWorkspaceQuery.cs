using Application;
using Application.DTO.Workspace;
using Application.Exceptions;
using Application.UseCases.Queries.User;
using DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Implementation.UseCases.Queries.Workspace
{
    public class EfGetWorkspaceQuery : EfUseCase, IGetWorkspaceQuery
    {
        public int Id => (int) UseCasesEnum.WorkspaceRetrieval;
        public UseCasesEnum Name => UseCasesEnum.WorkspaceRetrieval;
        public IApplicationActor _actor;

        public EfGetWorkspaceQuery(CustomContext context, IApplicationActor actor) : base(context)
        {
            _actor = actor;
        }

        public WorkspaceDto Execute(int workspaceId)
        {
            if (workspaceId <= 0)
            {
                throw new ArgumentException("Invalid workspace Id.");
            }

            // check if the workspace with that ID actually exists
            Domain.Workspace workspace = Context.Workspaces.Include(w => w.WorkspacesMedia)
                                                            .ThenInclude(wm => wm.Media)
                                                            .FirstOrDefault(w => w.Id == workspaceId); 
            if (workspace == null)
            {
                throw new EntityNotFoundException(nameof(Domain.Workspace), workspaceId);
            }

            // check if the user that initiated the query has the required usecase to view it (WorkspaceRetrieval)
            bool hasGetUsecase = _actor.WorkspacesUseCases.Any(wus => wus.WorkspaceId == workspaceId
                                        && wus.UseCaseIds.Any(us => (int)UseCasesEnum.WorkspaceRetrieval == us));

            if (!hasGetUsecase)
            {
                throw new EntityNotFoundException(nameof(Domain.Workspace), workspaceId);
            }

            return new ResponseWorkspaceDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Type = workspace.Type.ToString(),
                Contents = workspace.Contents,
                OwnerId = workspace.OwnerId,
                ParentId = workspace.ParentId,
                Images = workspace.WorkspacesMedia.Select(wm => wm.Media.Path).ToList(),
            };
        }
    }
}
