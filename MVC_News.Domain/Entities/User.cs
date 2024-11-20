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
}