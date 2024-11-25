using FluentValidation;
using MVC_News.MVC.DTOs.Contracts.Users.ChangePassword;

namespace MVC_News.MVC.Validators;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDTO>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .MinimumLength(8);
    }
}