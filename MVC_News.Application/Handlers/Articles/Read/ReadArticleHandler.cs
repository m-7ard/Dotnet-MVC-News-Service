using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Read;

public class ReadArticleHandler : IRequestHandler<ReadArticleQuery, OneOf<ReadArticleResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserRepository _userRespository;
    private readonly UserWithIdExistsValidatorAsync _userExistsValidatorAsync;
    private readonly ArticleExistsValidatorAsync _articleExistsValidatorAsync;

    public ReadArticleHandler(IArticleRepository articleRepository, IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _userRespository = userRepository;

        _userExistsValidatorAsync = new UserWithIdExistsValidatorAsync(userRepository);
        _articleExistsValidatorAsync = new ArticleExistsValidatorAsync(articleRepository);
    }

    public async Task<OneOf<ReadArticleResult, List<ApplicationError>>> Handle(ReadArticleQuery request, CancellationToken cancellationToken)
    {
        var articleExistsResult = await _articleExistsValidatorAsync.Validate(request.Id);
        if (articleExistsResult.TryPickT1(out var errors, out var article))
        {
            return errors;
        }

        var userExistsResult = await _userExistsValidatorAsync.Validate(request.UserId);
        if (userExistsResult.TryPickT1(out errors, out var user))
        {
            return errors;
        }

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

