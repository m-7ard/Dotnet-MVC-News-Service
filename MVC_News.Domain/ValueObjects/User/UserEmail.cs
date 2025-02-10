using OneOf;

namespace MVC_News.Domain.ValueObjects.User;

public class UserEmail : ValueObject
{
    private UserEmail(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static OneOf<bool, string> CanCreate(string value)
    {
        return true;
    }

    public static UserEmail ExecuteCreate(string value)
    {
        return new UserEmail(value);
    }

    public static OneOf<UserEmail, string> TryCreate(string value)
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