using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Delete;

public class DeleteArticleHandler : IRequestHandler<DeleteArticleCommand, OneOf<DeleteArticleResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserRepository _userRespository;
    private readonly UserWithIdExistsValidatorAsync _userExistsValidatorAsync;
    private readonly ArticleExistsValidatorAsync _articleExistsValidatorAsync;

    public DeleteArticleHandler(IArticleRepository articleRepository, IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _userRespository = userRepository;
        _userExistsValidatorAsync = new UserWithIdExistsValidatorAsync(userRepository);
        _articleExistsValidatorAsync = new ArticleExistsValidatorAsync(articleRepository);
    }

    public async Task<OneOf<DeleteArticleResult, List<ApplicationError>>> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
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

