using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using MVC_News.Domain.DomainFactories;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Update;

public class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand, OneOf<UpdateArticleResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserRepository _userRepository;
    
    private readonly UserWithIdExistsValidatorAsync _userExistsValidatorAsync;
    private readonly ArticleExistsValidatorAsync _articleExistsValidatorAsync;

    public UpdateArticleHandler(IArticleRepository articleRepository, IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
        
        _userExistsValidatorAsync = new UserWithIdExistsValidatorAsync(userRepository);
        _articleExistsValidatorAsync = new ArticleExistsValidatorAsync(articleRepository);
    }

    public async Task<OneOf<UpdateArticleResult, List<ApplicationError>>> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var articleExistsResult = await _articleExistsValidatorAsync.Validate(request.Id);
        if (articleExistsResult.TryPickT1(out var errors, out var article))
        {
            return errors;
        }

        var userExistsResult = await _userExistsValidatorAsync.Validate(request.AuthorId);
        if (userExistsResult.TryPickT1(out errors, out var user))
        {
            return errors;
        }

        if (!article.CanBeUpdatedBy(user))
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"User is not authorised to update this article.",
                    path: [],
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
            tags: request.Tags,
            isPremium: request.IsPremium
        );
        await _articleRepository.UpdateAsync(updatedArticle);

        return new UpdateArticleResult(
            article: updatedArticle
        );
    }
}