namespace MVC_News.Domain.Entities;

public class Subscription
{
    public Subscription(Guid id, Guid userId, DateTime startDate, DateTime expirationDate)
    {
        Id = id;
        UserId = userId;
        StartDate = startDate;
        ExpirationDate = expirationDate;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}