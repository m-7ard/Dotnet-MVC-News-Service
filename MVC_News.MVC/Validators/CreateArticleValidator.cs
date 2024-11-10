using FluentValidation;
using MVC_News.MVC.DTOs.Contracts.Articles.Create;

namespace MVC_News.MVC.Validators;

public class CreateArticleValidator : AbstractValidator<CreateArticleRequestDTO>
{
    public CreateArticleValidator()
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