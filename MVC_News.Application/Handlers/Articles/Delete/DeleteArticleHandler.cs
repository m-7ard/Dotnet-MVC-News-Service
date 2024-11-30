using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Delete;

public class DeleteArticleHandler : IRequestHandler<DeleteArticleCommand, OneOf<DeleteArticleResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserRepository _userRespository;

    public DeleteArticleHandler(IArticleRepository articleRepository, IUserRepository userRespository)
    {
        _articleRepository = articleRepository;
        _userRespository = userRespository;
    }

    public async Task<OneOf<DeleteArticleResult, List<ApplicationError>>> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(request.Id);
        if (article is null)
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"Article of id \"{request.Id}\" does not exist.",
                    path: ["_"],
                    code: ApplicationErrorCodes.ModelDoesNotExist
                )
            };
        }

        var user = await _userRespository.GetUserById(request.UserId);
        if (user == null)
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"User of id \"{request.UserId}\" does not exist.",
                    path: ["_"],
                    code: ApplicationErrorCodes.ModelDoesNotExist
                )
            };
        }

        var accessResult = article.CanBeDeletedBy(user);
        if (accessResult.IsT1)
        {
            return ApplicationErrorFactory.DomainErrorsToApplicationErrors(accessResult.AsT1);
        }

        await _articleRepository.DeleteAsync(article);
        return new DeleteArticleResult();
    }
}

