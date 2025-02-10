using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using MVC_News.Application.Validators.ArticleExistsValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Delete;

public class DeleteArticleHandler : IRequestHandler<DeleteArticleCommand, OneOf<DeleteArticleResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserExistsValidator<UserId> _userExistsValidatorAsync;
    private readonly IArticleExistsValidator<ArticleId> _articleExistsValidatorAsync;

    public DeleteArticleHandler(IArticleRepository articleRepository, IUserExistsValidator<UserId> userExistsValidatorAsync, IArticleExistsValidator<ArticleId> articleExistsValidatorAsync)
    {
        _articleRepository = articleRepository;
        _userExistsValidatorAsync = userExistsValidatorAsync;
        _articleExistsValidatorAsync = articleExistsValidatorAsync;
    }

    public async Task<OneOf<DeleteArticleResult, List<ApplicationError>>> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
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

        var canBeDeletedByUser = article.CanBeDeletedBy(user);
        if (!canBeDeletedByUser)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: "User does not have the permission to delete this article.",
                code: ApplicationErrorCodes.NotAllowed,
                path: []
            );
        }

        await _articleRepository.DeleteAsync(article);
        return new DeleteArticleResult();
    }
}

