using Application;
using Application.DTO.Workspace;
using Application.UseCases.Commands.Workspace;
using DataAccess;
using Domain;
using FluentValidation;
using Implementation.Validators.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.Commands.Workspace
{
    public class EfCreateWorkspaceCommand : EfUseCase, ICreateWorkspaceCommand
    {
        public int Id => (int) UseCasesEnum.WorkspaceCreation;
        public UseCasesEnum Name => UseCasesEnum.WorkspaceCreation;
        public WorkspaceValidator _validator;
        public IApplicationActor _actor { get; set; }

        public EfCreateWorkspaceCommand(CustomContext context, WorkspaceValidator validator, IApplicationActor actor) : base(context) 
        {
            _validator = validator;
            _actor = actor;
        }

        public void Execute(WorkspaceDto dto)
        {
            _validator.ValidateAndThrow(dto); 

            WorkspaceType workspaceType;

            if (!Enum.TryParse(dto.Type, true, out workspaceType))
            {
                throw new ArgumentException("Invalid workspace type provided.");
            }

            Context.Workspaces.Add(new Domain.Workspace
            {
                Name = dto.Name,
                Type = workspaceType,
                Contents = dto.Contents,
                OwnerId = _actor.Id,
                ParentId = dto.ParentId,
            });

            Context.SaveChanges();
        }
    }
}
