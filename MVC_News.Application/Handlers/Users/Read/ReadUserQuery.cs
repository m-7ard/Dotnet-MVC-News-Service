using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Read;

public class ReadUserQuery : IRequest<OneOf<ReadUserResult, List<ApplicationError>>>
{
    public ReadUserQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}