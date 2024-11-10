using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Authors.Read;

public class ReadAuthorQuery : IRequest<OneOf<ReadAuthorResult, List<PlainApplicationError>>>
{
    public ReadAuthorQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}