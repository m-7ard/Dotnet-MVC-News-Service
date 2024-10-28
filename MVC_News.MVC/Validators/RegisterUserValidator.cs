using FluentValidation;
using MVC_News.Application.DTOs.Users.Register;

namespace MVC_News.MVC.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequestDTO>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(64);
    }
}