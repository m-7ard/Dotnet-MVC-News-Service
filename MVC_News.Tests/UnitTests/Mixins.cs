using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests;

public class Mixins
{
    public static User CreateUser(int seed, bool isAdmin, List<Subscription> subscriptions)
    {
        var user = UserFactory.BuildNew(
            email: $"user_{seed}@mail.com",
            passwordHash: $"user_{seed}_passwordHash",
            displayName: $"user_{seed}",
            isAdmin: isAdmin,
            subscriptions: subscriptions
        );

        return user;
    }

    public static Article CreateArticle(int seed, Guid authorId, bool isPremium = false)
    {
        var article = ArticleFactory.BuildExisting(
            id: Guid.NewGuid(),
            title: $"article_{seed}",
            content: $"content_{seed}",
            headerImage: $"url/${seed}",
            dateCreated: DateTime.Now,
            authorId: authorId,
            tags: new List<string>() { $"tag_{seed}" },
            isPremium: isPremium
        );

        return article;
    }

    public static Subscription CreatedValidSubscription(Guid userId)
    {
        var startDate = DateTime.Now;
        var expirationDate = startDate.AddMonths(1);
        var article = SubscriptionFactory.BuildExisting(
            id: Guid.NewGuid(),
            userId: userId,
            startDate: startDate,
            expirationDate: expirationDate
        );

        return article;
    }

    public static Subscription CreatedExpiredSubscription(Guid userId)
    {
        var startDate = DateTime.Now;
        startDate = startDate.AddMonths(-2);
        var expirationDate = DateTime.Now;
        expirationDate = expirationDate.AddMonths(-1);
        var article = SubscriptionFactory.BuildExisting(
            id: Guid.NewGuid(),
            userId: userId,
            startDate: startDate,
            expirationDate: expirationDate
        );

        return article;
    }
}