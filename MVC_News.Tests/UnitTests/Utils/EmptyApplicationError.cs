using MVC_News.Application.Errors;

namespace MVC_News.Tests.UnitTests.Utils;

public static class EmptyApplicationError
{
    public static readonly ApplicationError Instance = new ApplicationError(message: "", code: "", path: []);
}
