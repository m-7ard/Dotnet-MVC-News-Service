using MVC_News.Domain.Entities;

namespace MVC_News.Domain.DomainFactories;

public class ArticleFactory
{
    public static Article BuildExisting(Guid id, string title, string content, string headerImage, DateTime dateCreated, Guid authorId, List<string> tags)
    {
        return new Article(
            id: id,
            title: title,
            content: content,
            headerImage: headerImage,
            dateCreated: dateCreated,
            authorId: authorId,
            tags: tags
        );
    }

    public static Article BuildNew(Guid id, string title, string content, string headerImage, Guid authorId, List<string> tags)
    {
        return new Article(
            id: id,
            title: title,
            content: content,
            headerImage: headerImage,
            dateCreated: new DateTime(),
            authorId: authorId,
            tags: tags
        );
    }
}