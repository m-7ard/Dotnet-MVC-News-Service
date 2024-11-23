using MVC_News.Domain.Errors;

namespace MVC_News.Application.Errors;

public class ApplicationDomainErrorMetadata
{
    public ApplicationDomainErrorMetadata(DomainError originalError)
    {
        OriginalError = originalError;
    }

    public DomainError OriginalError { get; }
}