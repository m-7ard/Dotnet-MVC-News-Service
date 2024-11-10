namespace MVC_News.MVC.Models.Articles;

public class CreateArticlePageModel : BaseViewModel
{
    public CreateArticlePageModel(string title, string content, Dictionary<string, List<string>> errors, string headerImage)
    {
        Title = title;
        Content = content;
        Errors = errors;
        HeaderImage = headerImage;
    }

    public string Title { get; }
    public string Content { get; }
    public string HeaderImage { get; }
    public Dictionary<string, List<string>> Errors { get; }
}