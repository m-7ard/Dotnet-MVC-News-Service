namespace MVC_News.MVC.Exceptions;

public class UnauthorizedException : ControllerException
{
    public UnauthorizedException(string message) : base(message, 401, "~/Views/401Unauthorized.cshtml")
    {
    }
}