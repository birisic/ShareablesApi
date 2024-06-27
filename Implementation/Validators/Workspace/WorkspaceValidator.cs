using Application;
using Application.DTO.Workspace;
using DataAccess;
using Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Validators.Workspace
{
    public class WorkspaceValidator : AbstractValidator<WorkspaceDto>
    {
        public CustomContext _context;
        public IApplicationActor _actor;

        /*
         *  - name - length, alphanumerics and some special chars like ()!-_'";
         *  - type - in WorkspaceTypesEnum
         *  - contents - nullable
         *  - parentId - nullable, > 0, if parentID does exist in the database and if the actor has the usecaseID 4 (WorkspaceCreation)
         *  AND if the actor has the "WorkspaceRetrieval" usecase for all ancestor workspaces of its parent workspace
         */

        public WorkspaceValidator(CustomContext context, IApplicationActor actor) 
        {
            _context = context;
            _actor = actor;
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(dto => dto.Name).NotEmpty()
                                    .Matches(@"^[a-zA-Z0-9\(\)!\-_'"";\s]*$")
                                    .WithMessage("Name must contain only alphanumeric characters or special chars ('()!-_'\"') and be between 1-50 characters long.")
                                    .Length(1, 50);

            RuleFor(dto => dto.Type).NotEmpty()
                                    .Must(t => Enum.IsDefined(typeof(WorkspaceType), t))
                                    .WithMessage("Workspace type must be 'Workspace', 'Directory' or 'Document'.");

            /*
             * when can an actor create a workspace or directory or document?
             * Workspace - whenever, but the type must be 'Workspace' and parentId must be NULL (it doesn't require any special 
             * usecases)
             * Directory and Document - when the actor has usecaseID 4 (WorkspaceCreation) in the parent workspace
             * AND when they have the "WorkspaceRetrieval" usecase for all ancestor workspaces
             */

            RuleFor(dto => dto.ParentId)
            .Must(ParentIdIsValid)
            .WithMessage("Invalid workspace parent id.");

            RuleFor(dto => dto)
                .Must(CanCreateWorkspace)
                .WithMessage("You do not have the necessary permissions to create this workspace.");
        }

        private bool ParentIdIsValid(int? parentId)
        {
            //allow nulls
            if (parentId == null) return true;

            if (parentId <= 0) return false;

            //allow only if a corresponding record exists
            var parentWorkspace = _context.Workspaces.FirstOrDefault(w => w.Id == parentId);
            return parentWorkspace != null;
        }

        private bool CanCreateWorkspace(WorkspaceDto dto)
        {
            //allow creating a new workspace of type 'Workspace' only if it doesn't have a parent (Workspace must be root)
            if (dto.Type == WorkspaceType.Workspace.ToString())
            {
                return !dto.ParentId.HasValue;
            }

            if (dto.ParentId.HasValue)
            {
                var parentWorkspace = _context.Workspaces.FirstOrDefault(w => w.Id == dto.ParentId);
                if (parentWorkspace == null) return false;

                // Check if the actor has WorkspaceRetrieval use case for all ancestor workspaces (meaning if the actor can even see
                // the workspace children in the tree structure on the UI)
                bool theyHaveRetrievalUseCase = DoAncestorWorkspacesHaveRetrieavalUsecase(parentWorkspace);
                if (!theyHaveRetrievalUseCase) return false;

                //allow the actor to create the workspace only if they have the "WorkspaceCreation" usecase for the parent workspace
                bool hasWorkspaceCreationUseCase = _actor.WorkspacesUseCases.Any(wus =>
                                                wus.WorkspaceId == dto.ParentId &&
                                                wus.UseCaseIds.Contains((int)UseCasesEnum.WorkspaceCreation));

                return hasWorkspaceCreationUseCase;
            }

            return false;
        }

        public bool DoAncestorWorkspacesHaveRetrieavalUsecase(Domain.Workspace parentWorkspace)
        {
            var actorWorkspaces = _actor.WorkspacesUseCases;
            var actorWorkspacesIds = actorWorkspaces.Select(wus => wus.WorkspaceId);
            var ancestorWorkspaces = _context.Workspaces.Where(w => actorWorkspacesIds.Contains(w.Id))
                                                        .Select(w => new Domain.Workspace
                                                        {
                                                            Name = w.Name,
                                                            Type = w.Type,
                                                            Contents = w.Contents,
                                                            OwnerId = w.OwnerId,
                                                            ParentId = w.ParentId
                                                        })
                                                        .ToList();
            //actorWorkspaces.Where(wus => wus.WorkspaceId == w.Id).Select(wus => wus.WorkspaceId).First()

            while (parentWorkspace != null)
            {
                bool hasWorkspaceRetrievalUseCase = actorWorkspaces.Any(wus =>
                    wus.WorkspaceId == parentWorkspace.Id &&
                    wus.UseCaseIds.Contains((int)UseCasesEnum.WorkspaceRetrieval));

                if (!hasWorkspaceRetrievalUseCase) return false;

                parentWorkspace = ancestorWorkspaces.FirstOrDefault(w => w.Id == parentWorkspace.ParentId);
            }

            return true;
        }
    }
}
