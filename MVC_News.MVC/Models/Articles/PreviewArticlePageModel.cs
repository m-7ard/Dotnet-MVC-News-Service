using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class PreviewArticlePageModel : BaseViewModel
{
    public PreviewArticlePageModel(ArticleDTO article, string markup, string next)
    {
        Article = article;
        Markup = markup;
        Next = next;
    }

    public string Next { get; }
    public ArticleDTO Article { get; }
    public string Markup { get; }
}