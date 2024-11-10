namespace MVC_News.MVC.Models.Partials;

public class CharFieldPartialViewModel
{
    public CharFieldPartialViewModel(string name, Dictionary<string, string> inputAttrs, string value)
    {
        Name = name;
        InputAttrs = inputAttrs;
        Value = value;
    }

    public string Name { get; set; }
    public string Value { get; set; }
    public Dictionary<string, string> InputAttrs { get; set; }
}