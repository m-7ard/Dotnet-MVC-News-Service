namespace MVC_News.Infrastructure.DbEntities;

public class UserDbEntity
{
    public UserDbEntity(Guid id, string email, string passwordHash, string displayName, bool isAdmin)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        IsAdmin = isAdmin;
    }

    public Guid Id { get; private set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string DisplayName { get; set; }
    public bool IsAdmin { get; set; }
}