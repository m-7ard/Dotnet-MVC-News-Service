namespace MVC_News.Infrastructure.DbEntities;

public class SubscriptionDbEntity
{
    public SubscriptionDbEntity(Guid id, Guid userId, DateTime startDate, DateTime expirationDate)
    {
        Id = id;
        UserId = userId;
        StartDate = startDate;
        ExpirationDate = expirationDate;
    }

    public Guid Id { get; private set; }

    // User FK
    public Guid UserId { get; set; }
    public UserDbEntity User { get; set; } = null!; 

    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}