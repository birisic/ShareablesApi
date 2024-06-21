using Application.DTO.User;
using DataAccess;
using FluentValidation;

namespace Implementation.Validators.User
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(CustomContext ctx)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Username)
                .NotEmpty()
                .Matches("(?=.{4,15}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$")
                .WithMessage("Invalid username format.")
                .Must(x => !ctx.Users.Any(u => u.Username == x && u.DeletedAt == null))
                .WithMessage("Username is already in use.");

            RuleFor(x => x.Password).NotEmpty().Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$")
                .WithMessage("Password must contain at least eight characters, one uppercase letter, one lowercase letter and one number:");
        }
    }
}
