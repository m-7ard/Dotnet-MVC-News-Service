namespace MVC_News.MVC.Models.Partials;

public class CheckboxFieldPartialViewModel
{
    public CheckboxFieldPartialViewModel(string name, bool value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public bool Value { get; set; }
}