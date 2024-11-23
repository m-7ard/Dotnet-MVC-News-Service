using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MVC_News.MVC.Exceptions;

namespace MVC_News.MVC.Controllers;

public class BaseController : Controller
{
    protected Guid TryReadUserIdFromClaims()
    {
        if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId)) {
            throw new UnauthorizedException($"User ID is missing from claims.");
        }

        return parsedUserId;
    }
}