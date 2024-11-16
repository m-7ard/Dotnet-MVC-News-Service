namespace MVC_News.MVC.Models.Articles;

public class UpdateArticlePageModel : BaseViewModel
{
    public UpdateArticlePageModel(string title, string content, Dictionary<string, List<string>> errors, string headerImage, Guid id, List<string> tags)
    {
        Title = title;
        Content = content;
        Errors = errors;
        HeaderImage = headerImage;
        Id = id;
        Tags = tags;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Content { get; }
    public string HeaderImage { get; }
    public List<string> Tags { get; }
    public Dictionary<string, List<string>> Errors { get; }
}