using MVC_News.Domain.Entities;

namespace MVC_News.Domain.DomainFactories;

public class SubscriptionFactory
{
    public static Subscription BuildExisting(Guid id, Guid userId, DateTime startDate, DateTime expirationDate)
    {
        return new Subscription(
            id: id,
            userId: userId,
            startDate: startDate,
            expirationDate: expirationDate
        );
    }

    public static Subscription BuildNew(Guid id, Guid userId, DateTime startDate, DateTime expirationDate)
    {
        return new Subscription(
            id: id,
            userId: userId,
            startDate: startDate,
            expirationDate: expirationDate
        );
    }
}