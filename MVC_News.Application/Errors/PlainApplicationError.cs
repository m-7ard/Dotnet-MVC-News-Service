namespace MVC_News.Application.Errors;

public class PlainApplicationError
{
    public string Message { get; set; }
    public string FieldName { get; set; }
    public string Code { get; set; }

    public PlainApplicationError(string message, string fieldName, string code)
    {
        Message = message;
        FieldName = fieldName;
        Code = code;
    }
}