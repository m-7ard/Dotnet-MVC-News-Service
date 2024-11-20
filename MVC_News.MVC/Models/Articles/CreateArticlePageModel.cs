namespace MVC_News.MVC.Models.Articles;

public class CreateArticlePageModel : BaseViewModel
{
    public CreateArticlePageModel(string title, string content, Dictionary<string, List<string>> errors, string headerImage, List<string> tags, bool isPremium)
    {
        Title = title;
        Content = content;
        Errors = errors;
        HeaderImage = headerImage;
        Tags = tags;
        IsPremium = isPremium;
    }

    public string Title { get; }
    public string Content { get; }
    public string HeaderImage { get; }
    public List<string> Tags { get; }
    public Dictionary<string, List<string>> Errors { get; }
    public bool IsPremium { get; }
}