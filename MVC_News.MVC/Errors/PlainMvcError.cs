namespace MVC_News.MVC.Errors;

public class PlainMvcError
{
    public string Message { get; set; }
    public string FieldName { get; set; }

    public PlainMvcError(string fieldName, string message)
    {
        Message = message;
        FieldName = fieldName;
    }
}