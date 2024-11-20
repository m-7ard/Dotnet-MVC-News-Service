using MediatR;
using MVC_News.Application.Contracts.Criteria;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.List;

public class ListArticlesHandler : IRequestHandler<ListArticlesQuery, OneOf<ListArticlesResult, List<ApplicationError>>>
{
    private readonly IArticleRepository _articleRepository;
    private readonly List<int> _allowedLimitBy = [24, 48, 72];

    public ListArticlesHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<OneOf<ListArticlesResult, List<ApplicationError>>> Handle(ListArticlesQuery request, CancellationToken cancellationToken)
    {
        Tuple<string, bool>? orderBy = new Tuple<string, bool>("DateCreated", false);
        if (request.OrderBy == "newest")
        {
            orderBy = new Tuple<string, bool>("DateCreated", false);
        }
        else if (request.OrderBy == "oldest")
        {
            orderBy = new Tuple<string, bool>("DateCreated", true);
        }

        var limitBy = 24;

        if (request.LimitBy is not null && !_allowedLimitBy.Contains(request.LimitBy.Value))
        {
            limitBy = request.LimitBy.Value;
        }

        var articles = await _articleRepository.FilterAllAsync(
            new FilterAllArticlesCriteria(
                authorId: request.AuthorId,
                createdAfter: request.CreatedAfter,
                createdBefore: request.CreatedBefore,
                orderBy: orderBy,
                limitBy: limitBy
            )
        );

        return new ListArticlesResult(articles: articles);
    }
}

