using Domain;
using Application.UseCases.Commands.User;
using Application.DTO.User;
using DataAccess;
using Implementation.Validators.User;
using FluentValidation;

namespace Implementation.UseCases.Commands.User
{
    public class EfRegisterUserCommand : EfUseCase, IRegisterUserCommand
    {
        public int Id => 1;
        public string Name => "UserRegistration";

        private RegisterUserDtoValidator _validator;

        public EfRegisterUserCommand(CustomContext context, RegisterUserDtoValidator validator)
            : base(context)
        {
            _validator = validator;
        }

        public void Execute(RegisterUserDto data)
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
                    new UserWorkspace { Workspace = workspace, UseCaseId = 3 },
                    new UserWorkspace { Workspace = workspace, UseCaseId = 4 },
                    new UserWorkspace { Workspace = workspace, UseCaseId = 5 }
                }
            };

            Context.Users.Add(user);

            Context.SaveChanges();
        }
    }
}
