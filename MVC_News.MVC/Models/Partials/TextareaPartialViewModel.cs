namespace MVC_News.MVC.Models.Partials;

public class TextareaPartialViewModel
{
    public TextareaPartialViewModel(string name, string value, Dictionary<string, string> textareaAttrs)
    {
        Name = name;
        Value = value;
        TextareaAttrs = textareaAttrs;
    }

    public string Name { get; }
    public string Value { get; }
    public Dictionary<string, string> TextareaAttrs { get; }
}