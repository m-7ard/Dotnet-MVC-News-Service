using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;
using OneOf;

namespace MVC_News.Application.Validators.ArticleExistsValidator;

public class ArticleExistsByIdValidator : IArticleExistsValidator<ArticleId>
{
    private readonly IArticleRepository _articleRepository;

    public ArticleExistsByIdValidator(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<OneOf<Article, List<ApplicationError>>> Validate(ArticleId id)
    {
        var Article = await _articleRepository.GetByIdAsync(id);

        if (Article is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"Article of Id \"{id}\" does not exist.",
                code: ApplicationValidatorErrorCodes.ARTICLE_EXISTS_ERROR,
                path: []
            );
        }

        return Article;
    }
}