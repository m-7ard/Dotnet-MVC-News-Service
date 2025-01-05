using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects;

namespace MVC_News.Domain.DomainFactories;

public class SubscriptionFactory
{
    public static Subscription BuildExisting(Guid id, Guid userId, SubscriptionDates subscriptionDates)
    {
        return new Subscription(
            id: id,
            userId: userId,
            subscriptionDates: subscriptionDates
        );
    }

    public static Subscription BuildNew(Guid id, Guid userId, SubscriptionDates subscriptionDates)
    {
        return new Subscription(
            id: id,
            userId: userId,
            subscriptionDates: subscriptionDates
        );
    }
}