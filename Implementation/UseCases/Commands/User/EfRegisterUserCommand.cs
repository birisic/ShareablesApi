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
        public int Id => 2;
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

            Domain.User user = new Domain.User
            {
                Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                Username = data.Username,
                UseCases = new List<UserUseCase>()
                {
                    new UserUseCase { UseCaseId = 3 },
                    new UserUseCase { UseCaseId = 5 }
                }
            };

            Context.Users.Add(user);

            Context.SaveChanges();
        }
    }
}
