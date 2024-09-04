using Application;
using Application.DTO.Workspace;
using DataAccess;
using Domain;
using FluentValidation;
using Implementation.UseCases;
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

            When(dto => UseCase != UseCasesEnum.WorkspaceDeletion, () =>
            {
                RuleFor(dto => dto.Name).NotEmpty()
                        .Matches(@"^[a-zA-Z0-9\(\)!\-_'"";\s]*$")
                        .WithMessage("Name must contain only alphanumeric characters or special chars ('()!-_'\"') and be between 1-50 characters long.")
                        .Length(1, 50);

                RuleFor(dto => dto.Type).NotEmpty()
                        .Must(t => Enum.IsDefined(typeof(WorkspaceType), t))
                        .WithMessage("Workspace type must be 'Workspace', 'Directory' or 'Document'.");
            });

            /*
             * when can an actor create a workspace or directory or document?
             * Workspace - whenever, but the type must be 'Workspace' and parentId must be NULL (it doesn't require any special 
             * usecases)
             * Directory and Document - when the actor has usecaseID 4 (WorkspaceCreation) in the parent workspace
             * AND when they have the "WorkspaceRetrieval" usecase for all ancestor workspaces
             */

            /* when can an actor update/delete a workspace or directory or document?
             * Workspace - can delete any but they must have at least one left, and only when they've no children workspaces
             * Directory and Document - when the actor has usecaseIDs 2,3,5 (WorkspaceDeletion, WorkspaceRetrieval,
             * WorkspaceModification) for the provided workspace AND when they have the "WorkspaceRetrieval" usecase 
             * for all ancestor workspaces
             */


            When(dto => dto.Type == WorkspaceType.Document.ToString(), () =>
            {
                RuleFor(dto => dto.Images)
                .Must(images => images == null
                || images.All(fileName =>
                {
                    var path = Path.Combine("wwwroot", "temp", fileName);
                    return File.Exists(path);
                }))
                .WithMessage("One or more files do not exist in the temporary folder.");
            });

            RuleFor(dto => dto.ParentId)
                .Must(ParentIdIsValid)
                .WithMessage("Invalid workspace parent id.");

            RuleFor(dto => dto)
                .Must(dto => CanActorPerformAction(dto, UseCase))
                .WithMessage("You don't have the necessary permissions to perform this action.");
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
            //allow creating/updating a workspace of type 'Workspace'
            if (UseCase == UseCasesEnum.WorkspaceCreation || UseCase == UseCasesEnum.WorkspaceModification)
            {
                if (dto.Type == WorkspaceType.Workspace.ToString())
                {
                    return !dto.ParentId.HasValue;
                }
            }

            int? workspaceId = dto.ParentId; //use parentid for checking ancestor use cases

            if (useCase != UseCasesEnum.WorkspaceCreation)
            {
                bool isTrueParent = _context.Workspaces.Any(w => w.Id == dto.Id && w.ParentId == dto.ParentId);
                
                if (!isTrueParent) return false;

                workspaceId = dto.Id;
                bool hasRetrievalUseCaseForCurrentWorkspace = _actor.WorkspacesUseCases
                                                                    .Any(wus => wus.WorkspaceId == workspaceId &&
                                                                    wus.UseCaseIds.Contains((int) UseCasesEnum.WorkspaceRetrieval));

                if (!hasRetrievalUseCaseForCurrentWorkspace) return false;
            }

            if (workspaceId.HasValue)
            {
                // find the workspace
                var workspace = _context.Workspaces.FirstOrDefault(w => w.Id == workspaceId);
                if (workspace == null || workspace.DeletedAt.HasValue) return false;

                // Check if the actor has WorkspaceRetrieval use case for all ancestor workspaces (meaning if the actor can even see
                // the workspace children in the tree structure on the UI)
                bool theyHaveRetrievalUseCase = workspace.DoAncestorWorkspacesHaveRetrieavalUseCase(_context, _actor);
                if (!theyHaveRetrievalUseCase) return false;

                //allow the actor to create/update/delete the workspace only if they have the parameter usecase
                bool hasWorkspaceUseCase = _actor.WorkspacesUseCases.Any(wus =>
                                                wus.WorkspaceId == workspaceId &&
                                                wus.UseCaseIds.Contains((int) useCase));

                return hasWorkspaceUseCase;
            }

            return false;
        }
    }
}
