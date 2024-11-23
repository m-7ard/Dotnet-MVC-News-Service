namespace MVC_News.Application.Errors;

public class ApplicationError
{
    public ApplicationError(string message, string code, List<string> path)
    {
        Message = message;
        Code = code;
        Path = path;
        Metadata = new Dictionary<string, object>();
    }


    public ApplicationError(string message, string code, List<string> path, object metadata)
    {
        Message = message;
        Code = code;
        Path = path;
        Metadata = metadata;
    }

    public string Message { get; set; }
    public List<string> Path { get; set; }
    public string Code { get; set; }

    public object Metadata { get; set; }
}