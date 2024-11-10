using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.List;

public class ListArticlesQuery : IRequest<OneOf<ListArticlesResult, List<PlainApplicationError>>>
{
    public ListArticlesQuery(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore)
    {
        AuthorId = authorId;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
    }

    public Guid? AuthorId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}