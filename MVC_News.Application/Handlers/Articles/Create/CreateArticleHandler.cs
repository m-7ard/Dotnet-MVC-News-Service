using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using MVC_News.Domain.DomainFactories;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Create;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, OneOf<CreateArticleResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly UserWithIdExistsValidatorAsync _userExistsValidatorAsync;

    public CreateArticleHandler(IUserRepository userRepository, IArticleRepository articleRepository)
    {
        _userRepository = userRepository;
        _articleRepository = articleRepository;
        _userExistsValidatorAsync = new UserWithIdExistsValidatorAsync(userRepository);
    }

    public async Task<OneOf<CreateArticleResult, List<ApplicationError>>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var userExistsResult = await _userExistsValidatorAsync.Validate(request.AuthorId);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            return errors;
        }

        if (!user.IsAdmin)
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"User is not authorised to create articles.",
                    path: ["_"],
                    code: ApplicationErrorCodes.NotAllowed
                )
            };
        }

        var article = await _articleRepository.CreateAsync(
            ArticleFactory.BuildNew(
                id: request.Id,
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                authorId: request.AuthorId,
                tags: request.Tags,
                isPremium: request.IsPremium
            )
        );

        return new CreateArticleResult(
            article: article
        );
    }
}