using Application;
using Application.DTO.User;
using Application.UseCases;
using Application.UseCases.Commands.User;
using DataAccess;
using Domain;
using FluentValidation;
using Implementation.Validators.User;

namespace Implementation.UseCases.Commands.User
{
    public class EfUpdateUserWorkspaceUseCaseCommand : EfUseCase, IUpdateUserWorkspaceUseCaseCommand
    {
        public int Id => (int) UseCasesEnum.UserWorkspaceUseCaseModification;
        public UseCasesEnum Name => UseCasesEnum.UserWorkspaceUseCaseModification;
        public UpdateUserWorkspaceUseCaseValidator _validator;

        public EfUpdateUserWorkspaceUseCaseCommand(CustomContext context, UpdateUserWorkspaceUseCaseValidator validator) : base(context)
        {
            _validator = validator;
        }

        public void Execute(UserWorkspaceUseCaseDto dto)
        {
            _validator.ValidateAndThrow(dto);

            if (dto.Action == UseCaseAction.Store.ToString())
            {
                Context.UsersWorkspaces.Add(new UserWorkspace
                {
                    UserId = dto.UserId,
                    WorkspaceId = dto.WorkspaceId,
                    UseCaseId = dto.UseCaseId,
                });
            }

            if (dto.Action == UseCaseAction.Delete.ToString())
            {
                UserWorkspace userWorkspace = new UserWorkspace
                {
                    UserId = dto.UserId,
                    WorkspaceId = dto.WorkspaceId,
                    UseCaseId = dto.UseCaseId
                };

                Context.UsersWorkspaces.Attach(userWorkspace);
                Context.UsersWorkspaces.Remove(userWorkspace);
            }

            Context.SaveChanges();
        }
    }
}
