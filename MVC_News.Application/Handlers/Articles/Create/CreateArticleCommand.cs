using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Create;

public class CreateArticleCommand : IRequest<OneOf<CreateArticleResult, List<PlainApplicationError>>>
{
    public CreateArticleCommand(Guid id, string title, string content, Guid authorId, string headerImage)
    {
        Id = id;
        Title = title;
        Content = content;
        AuthorId = authorId;
        HeaderImage = headerImage;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Content { get; }
    public string HeaderImage { get; }
    public Guid AuthorId { get; }
}