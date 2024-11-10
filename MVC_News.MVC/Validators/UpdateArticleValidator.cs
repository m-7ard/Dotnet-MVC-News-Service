using FluentValidation;
using MVC_News.MVC.DTOs.Contracts.Articles.Update;

namespace MVC_News.MVC.Validators;

public class UpdateArticleValidator : AbstractValidator<UpdateArticleRequestDTO>
{
    public UpdateArticleValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.HeaderImage)
            .NotEmpty()
            .MaximumLength(1028);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(10000);
    }
}