using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.Subscription;
using MVC_News.Domain.ValueObjects.User;

namespace MVC_News.Tests.UnitTests;

public class Mixins
{
    public static User CreateUser(int seed, bool isAdmin, List<Subscription> subscriptions)
    {
        var user = UserFactory.BuildNew(
            userId: UserId.NewUserId(),
            email: UserEmail.ExecuteCreate($"user_{seed}@mail.com"),
            passwordHash: $"user_{seed}_passwordHash",
            displayName: $"user_{seed}",
            isAdmin: isAdmin,
            subscriptions: subscriptions
        );

        return user;
    }

    public static Article CreateArticle(int seed, UserId authorId, bool isPremium = false)
    {
        var article = ArticleFactory.BuildExisting(
            id: ArticleId.NewArticleId(),
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

    public static Subscription CreatedValidSubscription(UserId userId)
    {
        var startDate = DateTime.UtcNow;
        var expirationDate = startDate.AddMonths(1);
        var subscriptionDates = SubscriptionDates.ExecuteCreate(
            startDate: startDate,
            expirationDate: expirationDate
        );

        var article = SubscriptionFactory.BuildExisting(
            id: Guid.NewGuid(),
            userId: userId,
            subscriptionDates: subscriptionDates
        );

        return article;
    }

    public static Subscription CreatedExpiredSubscription(UserId userId)
    {
        var startDate = DateTime.Now;
        startDate = startDate.AddMonths(-2);
        var expirationDate = DateTime.Now;
        expirationDate = expirationDate.AddMonths(-1);
        var subscriptionDates = SubscriptionDates.ExecuteCreate(
            startDate: startDate,
            expirationDate: expirationDate
        );

        var article = SubscriptionFactory.BuildExisting(
            id: Guid.NewGuid(),
            userId: userId,
            subscriptionDates: subscriptionDates
        );

        return article;
    }
}