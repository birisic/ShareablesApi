using Application.DTO.User;
using DataAccess;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Validators.User
{
    public class LoginRequestDtoValidator : AbstractValidator<UserAuthRequestDto>
    {
        public LoginRequestDtoValidator(CustomContext ctx)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Username)
                .NotEmpty()
                .Matches("(?=.{4,15}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$")
                .WithMessage("Invalid username format.");

            RuleFor(x => x.Password).NotEmpty().Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$")
                .WithMessage("Password must contain at least eight characters, one uppercase letter, one lowercase letter and one number:");
        }
    }
}
