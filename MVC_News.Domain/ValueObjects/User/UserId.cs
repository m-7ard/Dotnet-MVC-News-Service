using OneOf;

namespace MVC_News.Domain.ValueObjects.User;

public class UserId : ValueObject
{
    private UserId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static OneOf<bool, string> CanCreate(Guid value)
    {
        return true;
    }

    public static UserId ExecuteCreate(Guid value)
    {
        return new UserId(value);
    }

    public static UserId NewUserId()
    {
        return ExecuteCreate(Guid.NewGuid());
    }

    public static OneOf<UserId, string> TryCreate(Guid value)
    {
        var canCreateResult = CanCreate(value);
        if (canCreateResult.TryPickT1(out var error, out _))
        {
            return error;
        }

        return ExecuteCreate(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public override string ToString()
    {
        return Value.ToString();
    }
}