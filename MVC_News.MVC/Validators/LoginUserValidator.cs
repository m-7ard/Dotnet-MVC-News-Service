using FluentValidation;
using MVC_News.Application.DTOs.Users.Login;

namespace MVC_News.MVC.Validators;

public class LoginUserValidator : AbstractValidator<LoginUserRequestDTO>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}