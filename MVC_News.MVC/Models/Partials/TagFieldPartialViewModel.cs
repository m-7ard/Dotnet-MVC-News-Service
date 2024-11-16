namespace MVC_News.MVC.Models.Partials;

public class TagFieldPartialViewModel
{
    public TagFieldPartialViewModel(string name, List<string> value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public List<string> Value { get; set; }
}