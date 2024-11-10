using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Read;

public class ReadArticleQuery : IRequest<OneOf<ReadArticleResult, List<PlainApplicationError>>>
{
    public ReadArticleQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}