using OneOf;

namespace MVC_News.Domain.ValueObjects.Subscription;

public class SubscriptionDates : ValueObject
{
    public SubscriptionDates(DateTime startDate, DateTime expirationDate)
    {
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

    public static SubscriptionDates ExecuteCreate(DateTime startDate, DateTime expirationDate) {
        var canCreateResult = CanCreate(startDate: startDate, expirationDate: expirationDate);
        if (canCreateResult.TryPickT1(out var error, out var _))
        {
            throw new Exception(error);
        }

        return new SubscriptionDates(startDate: startDate, expirationDate: expirationDate);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return ExpirationDate;
    }
}