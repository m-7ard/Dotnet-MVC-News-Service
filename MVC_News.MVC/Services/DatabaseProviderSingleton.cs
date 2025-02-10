using MVC_News.Infrastructure.Interfaces;
using MVC_News.Infrastructure.Values;

namespace MVC_News.MVC.Services;

public class DatabaseProviderSingleton : IDatabaseProviderSingleton
{
    public DatabaseProviderSingletonValue Value { get; }
    public bool IsSQLite { get; }
    public bool IsMSSQL { get; }

    public DatabaseProviderSingleton(DatabaseProviderSingletonValue value)
    {
        Value = value;
        IsSQLite = value == DatabaseProviderSingletonValue.Sqlite;
        IsMSSQL = value == DatabaseProviderSingletonValue.SqlServer;

        if ((IsSQLite || IsMSSQL) is false)
        {
            throw new Exception($"\"{value}\" is not a valid database provider name.");
        }
    }
}