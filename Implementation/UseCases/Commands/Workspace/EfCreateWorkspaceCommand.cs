using Application;
using Application.DTO.Workspace;
using Application.UseCases.Commands.Workspace;
using DataAccess;
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

        public EfCreateWorkspaceCommand(CustomContext context, WorkspaceValidator validator) : base(context) 
        {
            _validator = validator;
        }

        public void Execute(WorkspaceDto data)
        {
            _validator.ValidateAndThrow(data);
        }
    }
}
