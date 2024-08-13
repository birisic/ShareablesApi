using Application;
using Application.DTO.Workspace;
using Application.UseCases.Commands.Workspace;
using DataAccess;
using Domain;
using FluentValidation;
using Implementation.Validators.Workspace;

namespace Implementation.UseCases.Commands.Workspace
{
    public class EfUpdateWorkspaceCommand : EfUseCase, IUpdateWorkspaceCommand
    {
        public int Id => (int) UseCasesEnum.WorkspaceModification;
        public UseCasesEnum Name => UseCasesEnum.WorkspaceModification;
        public WorkspaceDtoValidator _validator;
        public IApplicationActor _actor;

        public EfUpdateWorkspaceCommand(CustomContext context, IApplicationActor actor, WorkspaceDtoValidator validator)
            : base(context)
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

            var workspaceToUpdate = Context.Workspaces.FirstOrDefault(w => w.Id == dto.Id);

            if (workspaceToUpdate != null)
            {
                if (workspaceToUpdate.Name != dto.Name) workspaceToUpdate.Name = dto.Name;

                if (workspaceType == WorkspaceType.Document)
                {
                    if (workspaceToUpdate.Contents != dto.Contents) workspaceToUpdate.Contents = dto.Contents;
                }

                Context.SaveChanges();
            }
        }
    }
}
