using Application;
using Application.DTO.Workspace;
using DataAccess;
using Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Validators.Workspace
{
    public class WorkspaceDtoValidator : AbstractValidator<WorkspaceDto>
    {
        public CustomContext _context;
        public IApplicationActor _actor;
        public UseCasesEnum UseCase { get; set; }

        /*
         *  - name - length, alphanumerics and some special chars like ()!-_'";
         *  - type - in WorkspaceTypesEnum
         *  - contents - nullable
         *  - parentId - nullable, > 0, if parentID does exist in the database and if the actor has the usecaseID 4 (WorkspaceCreation)
         *  AND if the actor has the "WorkspaceRetrieval" usecase for all ancestor workspaces of its parent workspace
         */

        public WorkspaceDtoValidator(CustomContext context, IApplicationActor actor) 
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

            /* when can an actor update/delete a workspace or directory or document?
             * Workspace - can delete any but the first one and only when they've no children workspaces
             * Directory and Document - when the actor has usecaseIDs 2,3,5 (WorkspaceDeletion, WorkspaceRetrieval,
             * WorkspaceModification) for the provided workspace AND when they have the "WorkspaceRetrieval" usecase 
             * for all ancestor workspaces
             */

            RuleFor(dto => dto.ParentId)
            .Must(ParentIdIsValid)
            .WithMessage("Invalid workspace parent id.");

            RuleFor(dto => dto)
                .Must(dto => CanActorPerformAction(dto, UseCase))
                .WithMessage("You do not have the necessary permissions to perform this action.");
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

        private bool CanActorPerformAction(WorkspaceDto dto, UseCasesEnum useCase)
        {
            //allow creating a new workspace of type 'Workspace' only if it doesn't have a parent (Workspaces must be root)
            if (dto.Type == WorkspaceType.Workspace.ToString())
            {
                return !dto.ParentId.HasValue;
            }

            bool isTrueParent = _context.Workspaces.Any(w => w.Id == dto.Id && w.ParentId == dto.ParentId);

            if (!isTrueParent) return false; 

            int? workspaceId = dto.ParentId; //use parentid only for creating directories/documents

            if (useCase != UseCasesEnum.WorkspaceCreation)
            {
                workspaceId = dto.Id;
                bool hasRetrievalUseCaseForCurrentWorkspace = _actor.WorkspacesUseCases
                                                                    .Any(wus => wus.WorkspaceId == workspaceId &&
                                                                    wus.UseCaseIds.Contains((int) UseCasesEnum.WorkspaceRetrieval));

                if (!hasRetrievalUseCaseForCurrentWorkspace) return false;
            }

            if (workspaceId.HasValue)
            {
                var workspace = _context.Workspaces.FirstOrDefault(w => w.Id == workspaceId);
                if (workspace == null) return false;

                // Check if the actor has WorkspaceRetrieval use case for all ancestor workspaces (meaning if the actor can even see
                // the workspace children in the tree structure on the UI)
                bool theyHaveRetrievalUseCase = DoAncestorWorkspacesHaveRetrieavalUseCase(workspace);
                if (!theyHaveRetrievalUseCase) return false;

                //allow the actor to create the workspace only if they have the parameter usecase for the parent workspace
                bool hasWorkspaceUseCase = _actor.WorkspacesUseCases.Any(wus =>
                                                wus.WorkspaceId == workspaceId &&
                                                wus.UseCaseIds.Contains((int) useCase));

                return hasWorkspaceUseCase;
            }

            return false;
        }

        private bool DoAncestorWorkspacesHaveRetrieavalUseCase(Domain.Workspace workspace)
        {
            var actorWorkspaces = _actor.WorkspacesUseCases;
            var actorWorkspacesIds = actorWorkspaces.Select(wus => wus.WorkspaceId);
            var ancestorWorkspaces = _context.Workspaces.Where(w => actorWorkspacesIds.Contains(w.Id))
                                                        .Select(w => new Domain.Workspace
                                                        {
                                                            Id = w.Id,
                                                            //Name = w.Name,
                                                            //Type = w.Type,
                                                            //Contents = w.Contents,
                                                            //OwnerId = w.OwnerId,
                                                            ParentId = w.ParentId
                                                        })
                                                        .ToList();

            while (workspace != null)
            {
                bool hasWorkspaceRetrievalUseCase = actorWorkspaces.Any(wus =>
                    wus.WorkspaceId == workspace.Id &&
                    wus.UseCaseIds.Contains((int)UseCasesEnum.WorkspaceRetrieval));

                if (!hasWorkspaceRetrievalUseCase) return false;

                workspace = ancestorWorkspaces.FirstOrDefault(w => w.Id == workspace.ParentId);
            }

            return true;
        }
    }
}
