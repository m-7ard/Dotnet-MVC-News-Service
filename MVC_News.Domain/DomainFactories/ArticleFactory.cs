using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;

namespace MVC_News.Domain.DomainFactories;

public class ArticleFactory
{
    public static Article BuildExisting(ArticleId id, string title, string content, string headerImage, DateTime dateCreated, UserId authorId, List<string> tags, bool isPremium)
    {
        return new Article(
            id: id,
            title: title,
            content: content,
            headerImage: headerImage,
            dateCreated: dateCreated,
            authorId: authorId,
            tags: tags,
            isPremium: isPremium
        );
    }

    public static Article BuildNew(ArticleId id, string title, string content, string headerImage, DateTime dateCreated, UserId authorId, List<string> tags, bool isPremium)
    {
        return new Article(
            id: id,
            title: title,
            content: content,
            headerImage: headerImage,
            dateCreated: dateCreated,
            authorId: authorId,
            tags: tags,
            isPremium: isPremium
        );
    }
}