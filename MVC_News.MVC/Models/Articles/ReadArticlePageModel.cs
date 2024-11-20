using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class ReadArticlePageModel : BaseViewModel
{
    public ReadArticlePageModel(ArticleDTO article, string markup)
    {
        Article = article;
        Markup = markup;
    }

    public ArticleDTO Article { get; }
    public string Markup { get; }
}