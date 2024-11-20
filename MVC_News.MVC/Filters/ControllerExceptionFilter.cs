using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MVC_News.MVC.Exceptions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ControllerExceptionFilter : IExceptionFilter
{
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public ControllerExceptionFilter(IModelMetadataProvider modelMetadataProvider)
    {
        _modelMetadataProvider = modelMetadataProvider;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        if (exception is ControllerException controllerException)
        {
            var viewResult = new ViewResult
            {
                StatusCode = controllerException.StatusCode,
                ViewName = controllerException.ViewName
            };

            // Initialize ViewData with a valid IModelMetadataProvider and ModelState
            viewResult.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
            {
                Model = controllerException.Message
            };

            context.Result = viewResult;
            context.ExceptionHandled = true;
        }
    }
}