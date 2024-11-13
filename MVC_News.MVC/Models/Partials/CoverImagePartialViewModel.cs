namespace MVC_News.MVC.Models.Partials;

public class CoverImagePartialViewModel
{
    public CoverImagePartialViewModel(string className, string src)
    {
        ClassName = className;
        Src = src;
    }

    public string ClassName { get; set; }
    public string Src { get; set; }
}