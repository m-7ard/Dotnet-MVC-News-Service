namespace MVC_News.MVC.Exceptions;

public class ControllerException : Exception
{
    public int StatusCode { get; }
    public string ViewName { get; }

    public ControllerException(string message, int statusCode, string viewName) : base(message)
    {
        StatusCode = statusCode;
        ViewName = viewName;
    }
}