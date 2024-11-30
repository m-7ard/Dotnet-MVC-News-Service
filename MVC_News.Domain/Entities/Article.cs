using MVC_News.Domain.Errors;
using OneOf;
using OneOf.Types;

namespace MVC_News.Domain.Entities;

public class Article
{
    public Article(Guid id, string title, string content, DateTime dateCreated, string headerImage, Guid authorId, List<string> tags, bool isPremium)
    {
        Id = id;
        Title = title;
        Content = content;
        DateCreated = dateCreated;
        HeaderImage = headerImage;
        AuthorId = authorId;
        Tags = tags;
        IsPremium = isPremium;
    }

    public Guid Id { get; private set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string HeaderImage { get; set; }
    public DateTime DateCreated { get; set; }
    public Guid AuthorId { get; set; }
    public List<string> Tags { get; set; }
    public bool IsPremium { get; set; }

    public OneOf<bool, List<DomainError>> CanBeAccessedBy(User user) {
        if (!IsPremium)
        {
            return true;
        }

        if (user.IsAdmin)
        {
            return true;
        }

        if (user.HasActiveSubscription())
        {
            return true;
        }

        var errors = new List<DomainError>
        {
            new DomainError(
                message: "User does not have the permission to view this article.",
                path: new List<string>() { "_" },
                code: ArticleDomainErrorsCodes.UserNotAllowed
            )
        };

        return errors;
    }

    
    public OneOf<bool, List<DomainError>> CanBeDeletedBy(User user) {
        if (user.IsAdmin)
        {
            return true;
        }

        var errors = new List<DomainError>
        {
            new DomainError(
                message: "User does not have the permission to delete this article.",
                path: new List<string>() { "_" },
                code: ArticleDomainErrorsCodes.UserNotAllowed
            )
        };

        return errors;
    }
}