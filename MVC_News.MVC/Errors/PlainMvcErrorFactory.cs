using FluentValidation.Results;
using MVC_News.Application.Errors;
using OneOf.Types;

namespace MVC_News.MVC.Errors;

public static class PlainMvcErrorFactory
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

    public static Dictionary<string, List<string>> TranslateServiceErrors(List<ApplicationError> errors)
    {
        var result = new Dictionary<string, List<string>>();
        errors.ForEach((error) =>
        {
            var path = string.Join("/", error.Path);
            if (result.TryGetValue(path, out var fieldErrors))
            {
                fieldErrors.Add(error.Message);
            }
            else
            {
                result[path] = new List<string>() { error.Message };
            }
        });

        return result;
    }

    public static Dictionary<string, List<string>> MapApplicationErrors(List<ApplicationError> errors, Dictionary<string, List<string>>? codeDictionary = null, List<string>? defaultPath = null)
    {
        var result = new Dictionary<string, List<string>>();

        errors.ForEach((error) =>
        {
            List<string> finalPath = [..error.Path];

            if (codeDictionary is not null && codeDictionary.TryGetValue(error.Code, out var pathPrefix))
            {
                finalPath.InsertRange(0, pathPrefix);
            }
            else
            {
                finalPath = defaultPath ?? ["_"];
            }

            var path = string.Join("/", error.Path);
            if (result.TryGetValue(path, out var fieldErrors))
            {
                fieldErrors.Add(error.Message);
            }
            else
            {
                result[path] = new List<string>() { error.Message };
            }
        });

        return result;
    }
}