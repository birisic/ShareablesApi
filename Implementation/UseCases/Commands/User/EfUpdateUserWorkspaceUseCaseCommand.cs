using Application;
using Application.DTO.User;
using Application.UseCases.Commands.User;
using DataAccess;
using Domain;
using FluentValidation;
using Implementation.Validators.User;
using Microsoft.EntityFrameworkCore;

namespace Implementation.UseCases.Commands.User
{
    public class EfUpdateUserWorkspaceUseCaseCommand : EfUseCase, IUpdateUserWorkspaceUseCaseCommand
    {
        public int Id => (int) UseCasesEnum.UserWorkspaceUseCaseModification;
        public UseCasesEnum Name => UseCasesEnum.UserWorkspaceUseCaseModification;
        public UpdateUserWorkspaceUseCaseValidator _validator;

        public EfUpdateUserWorkspaceUseCaseCommand(CustomContext context, UpdateUserWorkspaceUseCaseValidator validator) 
            : base(context)
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

            if (dto.Action == UseCaseAction.Destroy.ToString())
            {
                UserWorkspace userWorkspace = new()
                {
                    UserId = dto.UserId,
                    WorkspaceId = dto.WorkspaceId,
                    UseCaseId = dto.UseCaseId
                };

                // if it's a WorkspaceRetrieval UseCase, remove other UseCases as well for this Workspace
                if (dto.UseCaseId != (int) UseCasesEnum.WorkspaceRetrieval)
                {
                    Context.UsersWorkspaces.Attach(userWorkspace);
                    Context.UsersWorkspaces.Remove(userWorkspace);
                }
                else
                {
                    var userWorkspacesToRemove = Context.UsersWorkspaces.Where(uw => uw.UserId == dto.UserId 
                                                                            && uw.WorkspaceId == dto.WorkspaceId)
                                                                        .ToList();

                    Context.UsersWorkspaces.RemoveRange(userWorkspacesToRemove);
                }
            }

            Context.SaveChanges();
        }
    }
}
