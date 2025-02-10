using MVC_News.Infrastructure.Values;

namespace Api.Services;

public static class BuilderUtils
{
    public class AppSettings
    {
        public DatabaseProviderSingletonValue DatabaseProviderValue { get; }
        public string ConnectionString { get; }

        public AppSettings(DatabaseProviderSingletonValue databaseProvidervalue, string connectionString)
        {
            DatabaseProviderValue = databaseProvidervalue;
            ConnectionString = connectionString;
        }
    }

    public static AppSettings ReadAppSettings(ConfigurationManager config)
    {
        var dbProvider = config["Database:Provider"];
        if (dbProvider is null)
        {
            throw new Exception("Database provider name cannot be null.");
        }

        var connectionString = config[$"{dbProvider}_Database"];
        if (connectionString is null)
        {
            throw new Exception("Connection string cannot be null.");
        }

        var dbProviderValue = DatabaseProviderSingletonValue.ExecuteCreate(dbProvider);

        return new AppSettings(databaseProvidervalue: dbProviderValue, connectionString: connectionString);
    }

    public static AppSettings ReadTestAppSettings(IConfiguration config)
    {
        var dbProvider = config["Testing:Database:Provider"];
        if (dbProvider is null)
        {
            throw new Exception("Database provider name cannot be null.");
        }

        var connectionString = config[$"Testing:{dbProvider}_Database"];
        if (connectionString is null)
        {
            throw new Exception("Connection string cannot be null.");
        }

        var dbProviderValue = DatabaseProviderSingletonValue.ExecuteCreate(dbProvider);

        return new AppSettings(databaseProvidervalue: dbProviderValue, connectionString: connectionString);
    }
}