using Application;
using Application.DTO.Workspace;
using Application.Exceptions;
using Application.UseCases.Commands.Workspace;
using DataAccess;
using FluentValidation;
using Implementation.Validators.Workspace;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Implementation.UseCases.Commands.Workspace
{
    public class EfDeleteWorkspaceCommand : EfUseCase, IDeleteWorkspaceCommand
    {
        public int Id => (int) UseCasesEnum.WorkspaceDeletion;
        public UseCasesEnum Name => UseCasesEnum.WorkspaceDeletion;
        public WorkspaceDtoValidator _validator;
        public IApplicationActor _actor;
        public EfDeleteWorkspaceCommand(CustomContext context, IApplicationActor actor, WorkspaceDtoValidator validator)
            : base(context)
        {
            _actor = actor;
            _validator = validator;
            _validator.UseCase = Name;
        }

        public void Execute(WorkspaceDto dto)
        {
            _validator.ValidateAndThrow(dto);

            var workspaceToDelete = Context.Workspaces.FirstOrDefault(w => w.Id == dto.Id) ??
                throw new EntityNotFoundException(nameof(Workspace), dto.Id ?? 0);

            if (workspaceToDelete.Children.Count > 0)
            {
                throw new ConflictException("The workspace couldn't be deleted because it contains subworkspaces.");
            }

            workspaceToDelete.DeletedAt = DateTime.UtcNow;
            Context.SaveChanges();
        }
    }
}
