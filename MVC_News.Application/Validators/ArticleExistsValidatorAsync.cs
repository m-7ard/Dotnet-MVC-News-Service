using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Application.Validators;

public class ArticleExistsValidatorAsync : IValidatorAsync<Guid, Article>
{
    private readonly IArticleRepository _articleRepository;

    public ArticleExistsValidatorAsync(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<OneOf<Article, List<ApplicationError>>> Validate(Guid input)
    {
        var article = await _articleRepository.GetByIdAsync(input);

        if (article is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"Article of id \"{input}\" does not exist.",
                code: ApplicationValidatorErrorCodes.ARTICLE_EXISTS_ERROR,
                path: []
            );
        }

        return article;
    }
}