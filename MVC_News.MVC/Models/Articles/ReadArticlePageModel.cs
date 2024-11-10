using MVC_News.Domain.Entities;

namespace MVC_News.MVC.Models.Articles;

public class ReadArticlePageModel : BaseViewModel
{
    public ReadArticlePageModel(Article article, string markup)
    {
        Article = article;
        Markup = markup;
    }

    public Article Article { get; }
    public string Markup { get; }
}