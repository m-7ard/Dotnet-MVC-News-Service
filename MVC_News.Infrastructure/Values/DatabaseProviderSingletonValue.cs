using MVC_News.Domain.ValueObjects;
using OneOf;

namespace MVC_News.Infrastructure.Values;

public class DatabaseProviderSingletonValue : ValueObject
{
    public static DatabaseProviderSingletonValue Sqlite => new DatabaseProviderSingletonValue("Sqlite");
    public static DatabaseProviderSingletonValue SqlServer => new DatabaseProviderSingletonValue("SqlServer");
    public static readonly List<DatabaseProviderSingletonValue> ValidValues = [Sqlite, SqlServer];

    public string Value { get; }

    private DatabaseProviderSingletonValue(string value)
    {   
        Value = value;
    }

    public static OneOf<DatabaseProviderSingletonValue, string> CanCreate(string value)
    {
        var status = ValidValues.Find(status => status.Value == value);

        if (status is null)
        {
            return $"{value} is not a valid DatabaseProviderSingletonValue";
        }

        return status;
    }

    public static DatabaseProviderSingletonValue ExecuteCreate(string value)
    {
        var canCreateResult = CanCreate(value);

        if (canCreateResult.TryPickT1(out var error, out var status))
        {
            throw new Exception(error);
        }

        return status;
    }

    public static bool IsValid(string status)
    {
        return ValidValues.Exists(d => d.Value == status);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}