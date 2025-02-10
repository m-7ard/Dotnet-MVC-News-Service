using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;

namespace MVC_News.Domain.DomainFactories;

public class UserFactory
{
    public static User BuildExisting(UserId id, UserEmail email, string passwordHash, string displayName, bool isAdmin, List<Subscription> subscriptions)
    {
        return new User(
            id: id,
            email: email,
            passwordHash: passwordHash,
            displayName: displayName,
            isAdmin: isAdmin,
            subscriptions: subscriptions
        );
    }

    public static User BuildNew(UserId userId, UserEmail email, string passwordHash, string displayName, bool isAdmin, List<Subscription> subscriptions)
    {
        return new User(
            id: userId,
            email: email,
            passwordHash: passwordHash,
            displayName: displayName,
            isAdmin: isAdmin,
            subscriptions: subscriptions
        );
    }
}