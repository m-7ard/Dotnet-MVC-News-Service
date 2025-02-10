namespace MVC_News.Application.Errors;

public class ApplicationErrorFactory
{
    public static List<ApplicationError> CreateSingleListError(string message, List<string> path, string code)
    {
        return new List<ApplicationError>()
        {
            new ApplicationError(
                message: message,
                path: path,
                code: code
            )
        };
    }
}