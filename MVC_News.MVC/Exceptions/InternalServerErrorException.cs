namespace MVC_News.MVC.Exceptions;

public class InternalServerErrorException : ControllerException
{
    public InternalServerErrorException(string message) : base(message, 500, "~/Views/500InternalServerError.cshtml")
    {
    }
}