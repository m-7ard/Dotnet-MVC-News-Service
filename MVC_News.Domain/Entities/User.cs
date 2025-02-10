using MVC_News.Domain.ValueObjects.Subscription;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Domain.Entities;

public class User
{
    public User(UserId id, UserEmail email, string passwordHash, string displayName, bool isAdmin, List<Subscription> subscriptions)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        IsAdmin = isAdmin;
        Subscriptions = subscriptions;
    }

    public UserId Id { get; private set; }
    public UserEmail Email { get; set; }
    public string PasswordHash { get; set; }
    public string DisplayName { get; set; }
    public bool IsAdmin { get; set; }
    public List<Subscription> Subscriptions { get; set; }

    public OneOf<bool, string> CanSubscribe(DateTime startDate, DateTime expirationDate)
    {
        if (HasActiveSubscription())
        {
            return "User is already subscribed";
        }

        var canCreateSubscriptionDatesResult = SubscriptionDates.CanCreate(startDate: startDate, expirationDate: expirationDate);
        if (canCreateSubscriptionDatesResult.TryPickT1(out var error, out var _))
        {
            return error;
        }

        return true;
    }

    public Guid ExcuteSubscribe(DateTime startDate, DateTime expirationDate)
    {
        var canSubscribeResult = CanSubscribe(startDate, expirationDate);
        if (canSubscribeResult.TryPickT1(out var error, out _))
        {
            throw new Exception(error);
        }

        var subscriptionDates = SubscriptionDates.ExecuteCreate(startDate: startDate, expirationDate: expirationDate);
        var subscription = new Subscription(
            id: Guid.NewGuid(), 
            userId: Id, 
            subscriptionDates: subscriptionDates
        );
        
        Subscriptions.Add(subscription);
        return subscription.Id;
    }

    public bool HasActiveSubscription()
    {
        return Subscriptions.Any(sub => sub.IsActive());
    }

    public Subscription? GetActiveSubscription()
    {
        return Subscriptions.Find(sub => sub.IsActive());
    }
}