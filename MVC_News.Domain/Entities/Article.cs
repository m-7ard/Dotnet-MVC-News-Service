using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;

namespace MVC_News.Domain.Entities;

public class Article
{
    public Article(ArticleId id, string title, string content, DateTime dateCreated, string headerImage, UserId authorId, List<string> tags, bool isPremium)
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

    public ArticleId Id { get; private set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string HeaderImage { get; set; }
    public DateTime DateCreated { get; set; }
    public UserId AuthorId { get; set; }
    public List<string> Tags { get; set; }
    public bool IsPremium { get; set; }

    public bool CanBeUpdatedBy(User user) {
        if (user.Id == AuthorId) {
            return true;
        }

        if (user.IsAdmin) {
            return true;
        }

        return false;
    } 

    public bool CanBeAccessedBy(User user) {
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

        return false;
    }

    
    public bool CanBeDeletedBy(User user) {
        if (user.IsAdmin)
        {
            return true;
        }

        return false;
    }
}