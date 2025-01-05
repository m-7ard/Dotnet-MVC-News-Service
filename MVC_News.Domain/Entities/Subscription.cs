using MVC_News.Domain.ValueObjects;

namespace MVC_News.Domain.Entities;

public class Subscription
{
    public Subscription(Guid id, Guid userId, SubscriptionDates subscriptionDates)
    {
        Id = id;
        UserId = userId;
        Dates = subscriptionDates;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; set; }
    public SubscriptionDates Dates { get; set; }

    public bool IsActive()
    {
        return Dates.ExpirationDate > DateTime.Now;
    }
}