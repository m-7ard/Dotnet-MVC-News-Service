using FluentValidation.Results;

namespace MVC_News.MVC.Errors;

public static class PlainApiErrorHandlingService
{
    public static Dictionary<string, List<string>> FluentToApiErrors(List<ValidationFailure> validationFailures)
    {
        var result = new Dictionary<string, List<string>>();
        validationFailures.ForEach((error) =>
        {
            if (result.TryGetValue(error.PropertyName, out var fieldErrors))
            {
                fieldErrors.Add(error.ErrorMessage);
            }
            else
            {
                result[error.PropertyName] = new List<string>() { error.ErrorMessage };
            }
        });

        return result;
    }

    public static Dictionary<string, List<string>> TranslateServiceErrors(List<PlainApplicationError> errors)
    {
        var result = new Dictionary<string, List<string>>();
        errors.ForEach((error) =>
        {
            if (result.TryGetValue(error.FieldName, out var fieldErrors))
            {
                fieldErrors.Add(error.Message);
            }
            else
            {
                result[error.FieldName] = new List<string>() { error.Message };
            }
        });

        return result;
    }
}