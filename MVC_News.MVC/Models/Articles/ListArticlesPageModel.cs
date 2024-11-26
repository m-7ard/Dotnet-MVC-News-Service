using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class ListArticlesPageModel : BaseViewModel
{
    public ListArticlesPageModel(List<ArticleDTO> articles, string tag)
    {
        Articles = articles;
        Tag = tag;
    }

    public List<ArticleDTO> Articles { get; }
    public string Tag { get; }
}