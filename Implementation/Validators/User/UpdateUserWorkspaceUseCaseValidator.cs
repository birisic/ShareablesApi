using FluentValidation;
using Application.DTO.Workspace;
using Application.DTO.User;
using DataAccess;
using Application;
using Domain;
using Implementation.UseCases;

namespace Implementation.Validators.User
{
    public class UpdateUserWorkspaceUseCaseValidator : AbstractValidator<UserWorkspaceUseCaseDto>
    {
        private CustomContext _context;
        private IApplicationActor _actor;
        public UpdateUserWorkspaceUseCaseValidator(CustomContext context, IApplicationActor actor)
        {
            /*
             * Validation checks:
             * 1. Ensure the actor has the UserWorkspaceUseCaseModification use case for the workspace.
             * 2. Verify that the user exists.
             * 3. Verify that the workspace exists.
             * 4. Ensure the action is correct and within UseCaseAction.
             * 5. If deleting, check if the user → workspace → use case combo exists.
             * 6. If storing, check if the user → workspace → use case combo doesn’t already exist.
             * 7. If adding a WorkspaceRetrieval usecase to a user on a workspace,
             *    you have to add it to all ancestor workspaces for that user as well, in order for it to be displayable in UI
             * 8. If adding WorkspaceDeletion or WorkspaceModification usecases, 
             *    make sure the user has the WorkspaceRetrieval usecase for that specific workspace, 
             *    but also for all the ancestor workspaces as well
             * 
             */
            _context = context;
            _actor = actor;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            RuleFor(dto => dto.WorkspaceId)
                            .NotEmpty()
                            .WithMessage("Workspace Id cannot be empty.")
                            .Must(workspaceId => context.Workspaces.Any(w => w.Id == workspaceId))
                            .WithMessage("Workspace with the provided ID does not exist.")
                            .Must(workspaceId => actor.WorkspacesUseCases.Any(wus => wus.WorkspaceId == workspaceId &&
                                    wus.UseCaseIds.Contains((int)UseCasesEnum.UserWorkspaceUseCaseModification)))
                            .WithMessage("You don't have permission to modify this workspace.");

            RuleFor(dto => dto.UserId)
                            .NotEmpty()
                            .WithMessage("User Id cannot be empty.")
                            .Must(userId => context.Users.Any(u => u.Id == userId))
                            .WithMessage("User with the provided ID does not exist.");

            RuleFor(dto => dto.Action)
                            .NotEmpty()
                            .WithMessage("Action type cannot be empty.")
                            .Must(action => Enum.IsDefined(typeof(UseCaseAction), action))
                            .WithMessage("Action type must be either 'Store' or 'Delete'.");

            When(dto => dto.Action == UseCaseAction.Delete.ToString(), () =>
            {
                RuleFor(dto => dto)
                    .Must(dto => context.Users
                                        .Where(u => u.Id == dto.UserId)
                                        .SelectMany(u => u.UsersWorkspaces)
                                        .Any(uw => uw.WorkspaceId == dto.WorkspaceId && uw.UseCaseId == dto.UseCaseId))
                    .WithMessage("Couldn't remove the user's privilege from the provided workspace " +
                        "since the user doesn't already have the provided privilege.")
                    .Must(dto => CanUpdateUseCase(dto));
            });

            When(dto => dto.Action == UseCaseAction.Store.ToString(), () =>
            {
                RuleFor(dto => dto)
                    .Must(dto => !context.Users
                                        .Where(u => u.Id == dto.UserId)
                                        .SelectMany(u => u.UsersWorkspaces)
                                        .Any(uw => uw.WorkspaceId == dto.WorkspaceId && uw.UseCaseId == dto.UseCaseId))
                    .WithMessage("Couldn't add the user's privilege to the provided workspace " +
                        "since the user already has the provided privilege.")
                    .Must(dto => CanUpdateUseCase(dto));
            });
        }

        public bool CanUpdateUseCase(UserWorkspaceUseCaseDto dto)
        {
            Domain.Workspace workspace = _context.Workspaces.FirstOrDefault(w => w.Id == dto.WorkspaceId);

            if (workspace == null) return false;

            var ancestorWorkspaces = GetAncestorWorkspaces(workspace);

            // Find ancestors where the actor does not have WorkspaceRetrieval use case
            var ancestorWorkspacesWithoutRetrievalUseCase = ancestorWorkspaces
                .Where(ancestor => !_actor.WorkspacesUseCases
                    .Any(wus => wus.WorkspaceId == ancestor.Id && wus.UseCaseIds.Contains((int)UseCasesEnum.WorkspaceRetrieval)))
                .ToList();

            if (dto.UseCaseId != (int)UseCasesEnum.WorkspaceRetrieval)
            {
                // If any ancestors are missing the WorkspaceRetrieval use case, and you're updating something other than
                // the WorkspaceRetrieval UseCase, then the action is not allowed
                // Otherwise, since you've already confirmed with the rules that the user has the UseCase for this workspace,
                // just allow them to perform the action
                return ancestorWorkspacesWithoutRetrievalUseCase.Count == 0;
            }
            else
            {
                if (dto.Action == UseCaseAction.Store.ToString())
                {
                    var userWorkspacesToAdd = new List<UserWorkspace>();

                    // Grant WorkspaceRetrieval for missing ancestors if the action is to grant it
                    foreach (var ancestor in ancestorWorkspacesWithoutRetrievalUseCase)
                    {
                        userWorkspacesToAdd.Add(CreateWorkspaceWithRetrievalUseCase(ancestor, dto.UserId));
                    }

                    _context.UsersWorkspaces.AddRange(userWorkspacesToAdd);
                    _context.SaveChanges();
                }
            }

            return true;
        }

        private List<Domain.Workspace> GetAncestorWorkspaces(Domain.Workspace workspace)
        {
            var ancestors = new List<Domain.Workspace>();

            var current = workspace;

            while (current.ParentId.HasValue)
            {
                var parent = _context.Workspaces.FirstOrDefault(w => w.Id == current.ParentId);
                if (parent != null)
                {
                    ancestors.Add(parent);
                    current = parent;
                }
                else
                {
                    break;
                }
            }

            return ancestors;
        }

        private static UserWorkspace CreateWorkspaceWithRetrievalUseCase(Domain.Workspace workspace, int userId)
        {
            return new()
            {
                WorkspaceId = workspace.Id,
                UserId = userId,
                UseCaseId = (int)UseCasesEnum.WorkspaceRetrieval
            };
        }
    }
}
