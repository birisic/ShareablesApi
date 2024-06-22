using Domain;
using Application.UseCases.Commands.User;
using Application.DTO.User;
using DataAccess;
using Implementation.Validators.User;
using FluentValidation;
using Application;

namespace Implementation.UseCases.Commands.User
{
    public class EfRegisterUserCommand : EfUseCase, IRegisterUserCommand
    {
        public int Id => (int)UseCasesEnum.UserRegistration;
        public UseCasesEnum Name => UseCasesEnum.UserRegistration;

        private RegisterUserDtoValidator _validator;

        public EfRegisterUserCommand(CustomContext context, RegisterUserDtoValidator validator)
            : base(context)
        {
            _validator = validator;
        }

        public void Execute(UserAuthRequestDto data)
        {
            _validator.ValidateAndThrow(data);

            Workspace workspace = new Workspace();

            Domain.User user = new Domain.User
            {
                Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                Username = data.Username,
                Workspaces = new List<Workspace> { workspace },
                UsersWorkspaces = new List<UserWorkspace>()
                {
                    new UserWorkspace { Workspace = workspace, UseCaseId = (int)UseCasesEnum.WorkspaceRetrieval },
                    new UserWorkspace { Workspace = workspace, UseCaseId = (int)UseCasesEnum.WorkspaceCreation },
                    new UserWorkspace { Workspace = workspace, UseCaseId = (int)UseCasesEnum.WorkspaceModification }
                }
            };

            Context.Users.Add(user);

            Context.SaveChanges();
        }
    }
}
