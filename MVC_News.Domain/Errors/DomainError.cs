namespace MVC_News.Domain.Errors;

public class DomainError
{
    public DomainError(string message, List<string> path, string code)
    {
        Message = message;
        Path = path;
        Code = code;
    }

    public string Message { get; set; }
    public List<string> Path { get; set; }
    public string Code { get; set; }
}