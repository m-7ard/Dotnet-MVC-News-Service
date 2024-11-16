using MVC_News.Domain.Entities;
using MVC_News.Infrastructure.DbEntities;

namespace MVC_News.Infrastructure.Mappers;

public static class ArticleMapper
{
    public static ArticleDbEntity FromDomainToDbEntity(Article source)
    {
        return new ArticleDbEntity(
            id: source.Id,
            title: source.Title,
            content: source.Content,
            headerImage: source.HeaderImage,
            dateCreated: source.DateCreated,
            authorId: source.AuthorId,
            tags: source.Tags.ToArray()
        );
    }

    public static Article FromDbEntityToDomain(ArticleDbEntity source)
    {
        return new Article(
            id: source.Id,
            title: source.Title,
            content: source.Content,
            headerImage: source.HeaderImage,
            dateCreated: source.DateCreated,
            authorId: source.AuthorId,
            tags: source.Tags.ToList()
        );
    }
}