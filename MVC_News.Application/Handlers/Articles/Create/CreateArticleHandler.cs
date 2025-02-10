using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Create;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, OneOf<CreateArticleResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserExistsValidator<UserId> _userExistsValidatorAsync;

    public CreateArticleHandler(IArticleRepository articleRepository, IUserExistsValidator<UserId> userExistsValidatorAsync)
    {
        _articleRepository = articleRepository;
        _userExistsValidatorAsync = userExistsValidatorAsync;
    }

    public async Task<OneOf<CreateArticleResult, List<ApplicationError>>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.TryCreate(request.AuthorId);
        if (userIdResult.TryPickT1(out var error, out var userId))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var userExistsResult = await _userExistsValidatorAsync.Validate(userId);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            return errors;
        }

        if (!user.IsAdmin)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User is not authorised to create articles.",
                path: ["_"],
                code: ApplicationErrorCodes.NotAllowed
            );
        }

        var article = ArticleFactory.BuildNew(
            id: ArticleId.NewArticleId(),
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            dateCreated: DateTime.UtcNow,
            authorId: user.Id,
            tags: request.Tags,
            isPremium: request.IsPremium
        );
        await _articleRepository.CreateAsync(article);

        return new CreateArticleResult(
            id: article.Id.Value
        );
    }
}