using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.List;

public class ListArticlesHandler : IRequestHandler<ListArticlesQuery, OneOf<ListArticlesResult, List<PlainApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;

    public ListArticlesHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<OneOf<ListArticlesResult, List<PlainApplicationError>>> Handle(ListArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _articleRepository.FilterAllAsync(
            createdAfter: request.CreatedAfter,
            createdBefore: request.CreatedBefore,
            authorId: request.AuthorId
        );

        return new ListArticlesResult(articles: articles);
    }
}

