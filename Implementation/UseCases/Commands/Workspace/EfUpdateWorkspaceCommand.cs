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

            Domain.Workspace workspaceToUpdate = Context.Workspaces.FirstOrDefault(w => w.Id == dto.Id);

            if (workspaceToUpdate != null)
            {
                if (workspaceToUpdate.Name != dto.Name) 
                {
                    dto.ValidateWorkspaceName(Context, workspaceType);
                    workspaceToUpdate.Name = dto.Name;
                }

                if (workspaceType == WorkspaceType.Document)
                {
                    if (workspaceToUpdate.Contents != dto.Contents) workspaceToUpdate.Contents = dto.Contents;

                    // Handle image files
                    if (dto.Images != null && dto.Images.Any())
                    {
                        var workspaceFolderPath = Path.Combine("wwwroot", "workspaces");

                        if (!Directory.Exists(workspaceFolderPath))
                        {
                            Directory.CreateDirectory(workspaceFolderPath);
                        }

                        foreach (var originalFileName in dto.Images)
                        {
                            var tempFilePath = Path.Combine("wwwroot", "temp", originalFileName);

                            if (File.Exists(tempFilePath))
                            {
                                var extension = originalFileName.Split('.')[1];
                                var newFileName = Guid.NewGuid().ToString() + '.' + extension;
                                var destinationFilePath = Path.Combine(workspaceFolderPath, newFileName);

                                File.Move(tempFilePath, destinationFilePath);

                                workspaceToUpdate.WorkspacesMedia.Add(new WorkspacesMedia
                                {
                                    Media = new Media
                                    {
                                        Path = newFileName,
                                        Name = originalFileName
                                    },
                                    Workspace = workspaceToUpdate
                                });
                            }
                            else 
                            { 
                                throw new FileNotFoundException($"An image with the name of '{originalFileName}'" +
                                $" was not found in the temporary media folder."); 
                            }
                        }
                    }

                    // if dto.Images == null, remove all records Media records associa
                }

                Context.SaveChanges();
            }
        }
    }
}
