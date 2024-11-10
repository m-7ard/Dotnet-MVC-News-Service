using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Update;

public class UpdateArticleCommand : IRequest<OneOf<UpdateArticleResult, List<PlainApplicationError>>>
{
    public UpdateArticleCommand(Guid id, string title, string content, string headerImage, Guid authorId)
    {
        Id = id;
        Title = title;
        Content = content;
        HeaderImage = headerImage;
        AuthorId = authorId;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Content { get; }
    public string HeaderImage { get; }

    public Guid AuthorId { get; }
}