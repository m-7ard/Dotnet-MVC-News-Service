using MVC_News.Domain.ValueObjects.Subscription;
using MVC_News.Domain.ValueObjects.User;

namespace MVC_News.Domain.Entities;

public class Subscription
{
    public Subscription(Guid id, UserId userId, SubscriptionDates subscriptionDates)
    {
        Id = id;
        UserId = userId;
        Dates = subscriptionDates;
    }

    public Guid Id { get; private set; }
    public UserId UserId { get; set; }
    public SubscriptionDates Dates { get; set; }

    public bool IsActive()
    {
        return Dates.ExpirationDate > DateTime.Now;
    }
}