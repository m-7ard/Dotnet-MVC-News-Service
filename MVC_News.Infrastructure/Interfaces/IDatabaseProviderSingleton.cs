using MVC_News.Infrastructure.Values;

namespace MVC_News.Infrastructure.Interfaces;

public interface IDatabaseProviderSingleton
{
    DatabaseProviderSingletonValue Value { get; }
    bool IsSQLite { get; }
    bool IsMSSQL { get; }
} 