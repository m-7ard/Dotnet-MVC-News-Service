using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Validators.ArticleExistsValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Read;

public class ReadArticleHandler : IRequestHandler<ReadArticleQuery, OneOf<ReadArticleResult, List<ApplicationError>>>
{
    private readonly IUserExistsValidator<UserId> _userExistsValidatorAsync;
    private readonly IArticleExistsValidator<ArticleId> _articleExistsValidator;

    public ReadArticleHandler(IUserExistsValidator<UserId> userExistsValidatorAsync, IArticleExistsValidator<ArticleId> articleExistsValidator)
    {
        _userExistsValidatorAsync = userExistsValidatorAsync;
        _articleExistsValidator = articleExistsValidator;
    }

    public async Task<OneOf<ReadArticleResult, List<ApplicationError>>> Handle(ReadArticleQuery request, CancellationToken cancellationToken)
    {
        // Article Exists
        var articleIdResult = ArticleId.TryCreate(request.Id);
        if (articleIdResult.TryPickT1(out var error, out var articleId))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }
        
        var articleExistsResult = await _articleExistsValidator.Validate(articleId);
        if (articleExistsResult.TryPickT1(out var errors, out var article))
        {
            return errors;
        }

        // User Exists
        var userIdResult = UserId.TryCreate(request.UserId);
        if (userIdResult.TryPickT1(out error, out var userId))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var userExistsResult = await _userExistsValidatorAsync.Validate(userId);
        if (userExistsResult.TryPickT1(out errors, out var user))
        {
            return errors;
        }

        // Can Access
        var canAccessArticle = article.CanBeAccessedBy(user);
        if (!canAccessArticle)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: "User does not have the permission to view this article.",
                path: [],
                code: ApplicationErrorCodes.NotAllowed
            );
        }

        return new ReadArticleResult(article: article);
    }
}

