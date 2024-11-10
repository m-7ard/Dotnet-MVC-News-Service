namespace MVC_News.MVC.Models.Partials;

public class SelectPartialiewModel
{
    public SelectPartialiewModel(string name, List<Tuple<string, object>> options, string placeholder)
    {
        Name = name;
        Options = options;
        Placeholder = placeholder;
    }

    public string Name { get; set; }
    public List<Tuple<string, object>> Options { get; set; }
    public string Placeholder { get; set; }
    public bool Nullable { get; set; }
}