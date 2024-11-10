using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Read;

public class ReadArticleHandler : IRequestHandler<ReadArticleQuery, OneOf<ReadArticleResult, List<PlainApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;

    public ReadArticleHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<OneOf<ReadArticleResult, List<PlainApplicationError>>> Handle(ReadArticleQuery request, CancellationToken cancellationToken)
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

        return new ReadArticleResult(article: article);
    }
}

