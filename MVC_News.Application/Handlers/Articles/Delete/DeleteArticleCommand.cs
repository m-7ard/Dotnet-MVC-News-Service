using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Delete;

public class DeleteArticleCommand : IRequest<OneOf<DeleteArticleResult, List<ApplicationError>>>
{
    public DeleteArticleCommand(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}