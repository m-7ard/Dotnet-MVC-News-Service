using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Partials;

public class ArticleContentPartialViewModel : BaseViewModel
{
    public ArticleContentPartialViewModel(ArticleDTO article, string markup)
    {
        Article = article;
        Markup = markup;
    }

    public ArticleDTO Article { get; }
    public string Markup { get; }
}