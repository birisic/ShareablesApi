using Application;
using Application.DTO.Workspace;
using Application.UseCases.Commands.Workspace;
using DataAccess;
using Domain;
using FluentValidation;
using Implementation.Validators.Workspace;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Implementation.UseCases.Commands.Workspace
{
    public class EfCreateWorkspaceCommand : EfUseCase, ICreateWorkspaceCommand
    {
        public int Id => (int) UseCasesEnum.WorkspaceCreation;
        public UseCasesEnum Name => UseCasesEnum.WorkspaceCreation;
        public WorkspaceDtoValidator _validator;
        public IApplicationActor _actor { get; set; }

        public EfCreateWorkspaceCommand(CustomContext context, IApplicationActor actor, WorkspaceDtoValidator validator) : base(context)
        {
            _actor = actor;
            _validator = validator;
            _validator.UseCase = Name;
        }

        public void Execute(WorkspaceDto dto)
        {
            _validator.ValidateAndThrow(dto);

            WorkspaceType workspaceType = Enum.Parse<WorkspaceType>(dto.Type);

            dto.ValidateWorkspaceName(Context, workspaceType);

            Domain.User user = Context.Users.FirstOrDefault(u => u.Username == _actor.Username);

            if (user != null)
            {
                Domain.Workspace workspace = new()
                {
                    Name = dto.Name,
                    Type = workspaceType,
                    Contents = dto.Contents,
                    OwnerId = _actor.Id,
                    ParentId = dto.ParentId,
                    UsersWorkspaces = new List<UserWorkspace>()
                    {
                        new UserWorkspace { User = user, UseCaseId = (int)UseCasesEnum.WorkspaceRetrieval },
                        new UserWorkspace { User = user, UseCaseId = (int)UseCasesEnum.WorkspaceModification },
                        new UserWorkspace { User = user, UseCaseId = (int)UseCasesEnum.WorkspaceDeletion },
                        new UserWorkspace { User = user, UseCaseId = (int)UseCasesEnum.UserWorkspaceUseCaseModification }
                    }
                };

                if (workspace.Type == WorkspaceType.Directory)
                {
                    workspace.UsersWorkspaces.Add(new UserWorkspace
                    { User = user, UseCaseId = (int)UseCasesEnum.WorkspaceCreation });
                }
                Context.Workspaces.Add(workspace);

                Context.SaveChanges();
            }
        }
    }
}
