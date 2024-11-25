using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.List;

public class ListArticlesQuery : IRequest<OneOf<ListArticlesResult, List<ApplicationError>>>
{
    public ListArticlesQuery(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore, string? orderBy, int? limitBy, List<string>? tags)
    {
        AuthorId = authorId;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
        OrderBy = orderBy;
        LimitBy = limitBy;
        Tags = tags;
    }

    public Guid? AuthorId { get; }
    public DateTime? CreatedAfter { get; }
    public DateTime? CreatedBefore { get; }
    public string? OrderBy { get; }
    public int? LimitBy { get; }
    public List<string>? Tags { get; }
}