namespace MVC_News.MVC.Exceptions;

public class NotFoundException : ControllerException
{
    public NotFoundException(string message) : base(message, 404, "~/Views/404NotFound.cshtml")
    {
    }
}