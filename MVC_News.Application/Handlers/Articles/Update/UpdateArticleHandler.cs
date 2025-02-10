using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using MVC_News.Application.Validators.ArticleExistsValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Update;

public class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand, OneOf<UpdateArticleResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserExistsValidator<UserId> _userExistsValidatorAsync;
    private readonly IArticleExistsValidator<ArticleId> _articleExistsValidatorAsync;

    public UpdateArticleHandler(IArticleRepository articleRepository, IUserExistsValidator<UserId> userExistsValidatorAsync, IArticleExistsValidator<ArticleId> articleExistsValidatorAsync)
    {
        _articleRepository = articleRepository;
        _userExistsValidatorAsync = userExistsValidatorAsync;
        _articleExistsValidatorAsync = articleExistsValidatorAsync;
    }

    public async Task<OneOf<UpdateArticleResult, List<ApplicationError>>> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        // Article exists
        var articleIdResult = ArticleId.TryCreate(request.Id);
        if (articleIdResult.TryPickT1(out var error, out var articleId))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var articleExistsResult = await _articleExistsValidatorAsync.Validate(articleId);
        if (articleExistsResult.TryPickT1(out var errors, out var article))
        {
            return errors;
        }

        // User exists
        var userIdResult = UserId.TryCreate(request.AuthorId);
        if (userIdResult.TryPickT1(out error, out var userId))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var userExistsResult = await _userExistsValidatorAsync.Validate(userId);
        if (userExistsResult.TryPickT1(out errors, out var user))
        {
            return errors;
        }

        // Can be deleted
        if (!article.CanBeUpdatedBy(user))
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User is not authorised to update this article.",
                path: [],
                code: ApplicationErrorCodes.NotAllowed
            );
        }

        var updatedArticle = ArticleFactory.BuildExisting(
            id: articleId,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            dateCreated: article.DateCreated,
            authorId: article.AuthorId,
            tags: request.Tags,
            isPremium: request.IsPremium
        );
        await _articleRepository.UpdateAsync(updatedArticle);

        return new UpdateArticleResult(
            article: updatedArticle
        );
    }
}