using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Articles.Create;

public class CreateArticleCommand : IRequest<OneOf<CreateArticleResult, List<ApplicationError>>>
{
    public CreateArticleCommand(Guid id, string title, string content, Guid authorId, string headerImage, List<string> tags, bool isPremium)
    {
        Id = id;
        Title = title;
        Content = content;
        AuthorId = authorId;
        HeaderImage = headerImage;
        Tags = tags;
        IsPremium = isPremium;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Content { get; }
    public string HeaderImage { get; }
    public Guid AuthorId { get; }
    public List<string> Tags { get; }
    public bool IsPremium { get; }
}