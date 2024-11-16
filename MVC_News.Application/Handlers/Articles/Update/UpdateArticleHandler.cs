using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.DomainFactories;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Update;

public class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand, OneOf<UpdateArticleResult, List<PlainApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserRepository _userRepository;

    public UpdateArticleHandler(IArticleRepository articleRepository, IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
    }

    public async Task<OneOf<UpdateArticleResult, List<PlainApplicationError>>> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(request.Id);
        if (article is null)
        {
            return new List<PlainApplicationError>()
            {
                new PlainApplicationError(
                    message: $"Article of id \"{request.Id}\" does not exist.",
                    fieldName: "_",
                    code: ApplicationErrorCodes.ModelDoesNotExist
                )
            };
        }

        var user = await _userRepository.GetUserById(request.AuthorId);
        if (user is null)
        {
            return new List<PlainApplicationError>()
            {
                new PlainApplicationError(
                    message: $"User of id \"{request.Id}\" does not exist.",
                    fieldName: "_",
                    code: ApplicationErrorCodes.ModelDoesNotExist
                )
            };
        }

        if (!user.IsAdmin)
        {
            return new List<PlainApplicationError>()
            {
                new PlainApplicationError(
                    message: $"User is not authorised to update articles.",
                    fieldName: "_",
                    code: ApplicationErrorCodes.NotAllowed
                )
            };
        }

        var updatedArticle = ArticleFactory.BuildExisting(
            id: request.Id,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            dateCreated: article.DateCreated,
            authorId: article.AuthorId,
            tags: request.Tags
        );
        await _articleRepository.UpdateAsync(updatedArticle);

        return new UpdateArticleResult(
            article: updatedArticle
        );
    }
}