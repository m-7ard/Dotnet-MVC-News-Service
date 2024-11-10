using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC_News.MVC.Models;

namespace MVC_News.MVC.Filters;

public class GlobalDataInjectorFilter : IActionFilter
{
    // private readonly ILocalCategoryService _localCategoryService;

    public GlobalDataInjectorFilter()
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // This method is called before the action executes
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Controller is Controller controller)
        {
            if (controller.ViewData.Model is BaseViewModel model)
            {
                // var baseCategory = _localCategoryService.GetBaseCategory();
                // model.BaseCategory = baseCategory;
                // model.OneDeepCategories = baseCategory is null ? [] : _localCategoryService.GetSubcategoriesById(baseCategory.Id);
            }
        }

    }
}
