using MVC_News.Domain.Errors;
using MVC_News.Domain.ValueObjects;
using OneOf;

namespace MVC_News.Domain.Entities;

public class User
{
    public User(Guid id, string email, string passwordHash, string displayName, bool isAdmin, List<Subscription> subscriptions)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        IsAdmin = isAdmin;
        Subscriptions = subscriptions;
    }

    public Guid Id { get; private set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string DisplayName { get; set; }
    public bool IsAdmin { get; set; }
    public List<Subscription> Subscriptions { get; set; }

    public OneOf<bool, string> CanSubscribe(DateTime expirationDate)
    {
        if (HasActiveSubscription())
        {
            return "User is already subscribed";
        }


        var startDate = DateTime.Now;
        var canCreateSubscriptionDatesResult = SubscriptionDates.CanCreate(startDate: startDate, expirationDate: expirationDate);
        if (canCreateSubscriptionDatesResult.TryPickT1(out var error, out var _))
        {
            return error;
        }

        var subscriptionDates = new SubscriptionDates(startDate: startDate, expirationDate: expirationDate);

        var subscription = new Subscription(
            id: Guid.NewGuid(), 
            userId: Id, 
            subscriptionDates: subscriptionDates
        );
        
        Subscriptions.Add(subscription);
        
        return true;
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