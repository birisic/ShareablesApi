using FluentValidation;
using Application.DTO.Workspace;
using Application.DTO.User;
using DataAccess;
using Application;

namespace Implementation.Validators.User
{
    public class UpdateUserWorkspaceUseCaseValidator : AbstractValidator<UserWorkspaceUseCaseDto>
    {
        private readonly CustomContext _context;
        private readonly IApplicationActor _actor;

        public UpdateUserWorkspaceUseCaseValidator(CustomContext context, IApplicationActor actor)
        {
            _context = context;
            _actor = actor;

            /*
             * Validation checks:
             * 1. Ensure the actor has the UserWorkspaceUseCaseModification use case for the workspace.
             * 2. Verify that the user exists.
             * 3. Verify that the workspace exists.
             * 4. Ensure the action is correct and within UseCaseAction.
             * 5. If deleting, check if the user → workspace → use case combo exists.
             * 6. If storing, check if the user → workspace → use case combo doesn’t already exist.
             */

            CascadeMode = CascadeMode.StopOnFirstFailure;


            RuleFor(dto => dto.WorkspaceId)
                            .NotEmpty()
                            .WithMessage("Workspace Id cannot be empty.")
                            .Must(workspaceId => actor.WorkspacesUseCases.Any(wus =>
                                wus.WorkspaceId == workspaceId && wus.UseCaseIds.Contains((int)UseCasesEnum.UserWorkspaceUseCaseModification)))
                            .WithMessage("You don't have permission to modify this workspace.");

            RuleFor(dto => dto.UserId)
                            .NotEmpty()
                            .WithMessage("User Id cannot be empty.")
                            .Must(userId => context.Users.Any(u => u.Id == userId))
                            .WithMessage("User with the provided ID does not exist.");

            RuleFor(dto => dto.WorkspaceId)
                            .Must(workspaceId => context.Workspaces.Any(w => w.Id == workspaceId))
                            .WithMessage("Workspace with the provided ID does not exist.");

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
                                        "since the user doesn't already have the provided privilege.");
            });

            When(dto => dto.Action == UseCaseAction.Store.ToString(), () =>
            {
                RuleFor(dto => dto)
                    .Must(dto => !context.Users
                                        .Where(u => u.Id == dto.UserId)
                                        .SelectMany(u => u.UsersWorkspaces)
                                        .Any(uw => uw.WorkspaceId == dto.WorkspaceId && uw.UseCaseId == dto.UseCaseId))
                                        .WithMessage("Couldn't add the user's privilege to the provided workspace " +
                                         "since the user already has the provided privilege.");
            });
        }
    }
}
