using MVC_News.Domain.Errors;
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

    public OneOf<bool, List<DomainError>> Subscribe(DateTime expirationDate)
    {
        if (HasActiveSubscription())
        {
            return new List<DomainError>
            {
                new DomainError(
                    message: "User is already subscribed",
                    path: new List<string>() { "_" },
                    code: UserDomainErrorCodes.UserAlreadySubscribed
                )
            };
        }

        var subscription = new Subscription(
            id: Guid.NewGuid(), 
            userId: Id, 
            startDate: DateTime.Now, 
            expirationDate: expirationDate
        );
        Subscriptions.Add(subscription);
        
        return true;
    }

    public bool HasActiveSubscription()
    {
        return Subscriptions.Any(sub => sub.ExpirationDate > DateTime.Now);
    }

    public Subscription? GetActiveSubscription()
    {
        return Subscriptions.Find(sub => sub.ExpirationDate > DateTime.Now);
    }
}