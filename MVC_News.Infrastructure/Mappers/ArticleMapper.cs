using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Infrastructure.DbEntities;

namespace MVC_News.Infrastructure.Mappers;

public static class ArticleMapper
{
    public static ArticleDbEntity FromDomainToDbEntity(Article source)
    {
        return new ArticleDbEntity(
            id: source.Id.Value,
            title: source.Title,
            content: source.Content,
            headerImage: source.HeaderImage,
            dateCreated: source.DateCreated,
            authorId: source.AuthorId.Value,
            tags: source.Tags.ToArray(),
            isPremium: source.IsPremium
        );
    }

    public static Article FromDbEntityToDomain(ArticleDbEntity source)
    {
        return new Article(
            id: ArticleId.ExecuteCreate(source.Id),
            title: source.Title,
            content: source.Content,
            headerImage: source.HeaderImage,
            dateCreated: source.DateCreated,
            authorId: UserId.ExecuteCreate(source.AuthorId),
            tags: source.Tags.ToList(),
            isPremium: source.IsPremium
        );
    }
}