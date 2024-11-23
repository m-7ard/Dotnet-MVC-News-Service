using Microsoft.AspNetCore.Mvc;
using MVC_News.MVC.Exceptions;

namespace MVC_News.MVC.Controllers;

public class StatusController : BaseController
{
    [HttpGet("unauthorised")]
    public IActionResult UnauthorisedPage()
    {
        throw new UnauthorizedException("User is unauthorised to access this resource.");
    }
}
