using OneOf;

namespace MVC_News.Domain.ValueObjects;

public class SubscriptionDates
{
    public SubscriptionDates(DateTime startDate, DateTime expirationDate)
    {
        var canCreateResult = CanCreate(startDate: startDate, expirationDate: expirationDate);
        if (canCreateResult.TryPickT1(out var error, out var _))
        {
            throw new Exception(error);
        }

        StartDate = startDate;
        ExpirationDate = expirationDate;
    }

    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    public static OneOf<bool, string> CanCreate(DateTime startDate, DateTime expirationDate) {
        if (startDate > expirationDate)
        {
            return "Subscription start date cannot be smaller than expiration date";
        }

        return true;
    }
}