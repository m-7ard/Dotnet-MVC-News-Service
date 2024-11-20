using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Read;

public class ReadArticleQuery : IRequest<OneOf<ReadArticleResult, List<ApplicationError>>>
{
    public ReadArticleQuery(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}