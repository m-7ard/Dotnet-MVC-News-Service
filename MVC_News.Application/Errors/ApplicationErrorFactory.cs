using MVC_News.Domain.Errors;

namespace MVC_News.Application.Errors;

public class ApplicationErrorFactory
{
    public static List<ApplicationError> DomainErrorsToApplicationErrors(List<DomainError> errors, List<string>? pathPrefix = null)
    {
        pathPrefix = pathPrefix ?? new List<string>();

        var applicationErrors = new List<ApplicationError>();

        foreach (var error in errors)
        {
            var applicationError = new ApplicationError(
                message: error.Message,
                path: new List<string>(pathPrefix.Concat(error.Path)),
                code: ApplicationErrorCodes.DomainError,
                metadata: new ApplicationDomainErrorMetadata(originalError: error)
            );
            applicationErrors.Add(applicationError);
        }

        return applicationErrors;
    }

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