using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests;

public class Mixins
{
    public static User CreateUser(int seed, bool isAdmin)
    {
        var user = UserFactory.BuildNew(
            email: $"user_{seed}@mail.com",
            passwordHash: $"user_{seed}_passwordHash",
            displayName: $"user_{seed}",
            isAdmin: isAdmin,
            subscriptions: []
        );

        return user;
    }

    public static Article CreateArticle(int seed, Guid authorId)
    {
        var article = ArticleFactory.BuildExisting(
            id: Guid.NewGuid(),
            title: $"article_{seed}",
            content: $"content_{seed}",
            headerImage: $"url/${seed}",
            dateCreated: DateTime.Now,
            authorId: authorId,
            tags: new List<string>() { $"tag_{seed}" }
        );

        return article;
    }
}