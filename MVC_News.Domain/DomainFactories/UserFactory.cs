using MVC_News.Domain.Entities;

namespace MVC_News.Domain.DomainFactories;

public class UserFactory
{
    public static User BuildExisting(Guid id, string email, string passwordHash, string displayName, bool isAdmin, List<Subscription> subscriptions)
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

    public static User BuildNew(string email, string passwordHash, string displayName, bool isAdmin, List<Subscription> subscriptions)
    {
        return new User(
            id: Guid.NewGuid(),
            email: email,
            passwordHash: passwordHash,
            displayName: displayName,
            isAdmin: isAdmin,
            subscriptions: subscriptions
        );
    }
}