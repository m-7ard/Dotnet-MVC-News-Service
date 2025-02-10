using MVC_News.Application.Errors;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Application.Validators.ArticleExistsValidator;

public interface IArticleExistsValidator<InputType> 
{
    public Task<OneOf<Article, List<ApplicationError>>> Validate(InputType input);
}