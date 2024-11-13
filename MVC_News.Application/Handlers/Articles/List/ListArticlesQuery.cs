using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.List;

public class ListArticlesQuery : IRequest<OneOf<ListArticlesResult, List<PlainApplicationError>>>
{
    public ListArticlesQuery(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore, string? orderBy, int? limitBy)
    {
        AuthorId = authorId;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
        OrderBy = orderBy;
        LimitBy = limitBy;
    }

    public Guid? AuthorId { get; }
    public DateTime? CreatedAfter { get; }
    public DateTime? CreatedBefore { get; }
    public string? OrderBy { get; }
    public int? LimitBy { get; }
}