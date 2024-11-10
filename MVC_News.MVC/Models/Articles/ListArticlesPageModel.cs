using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class ListArticlesPageModel : BaseViewModel
{
    public ListArticlesPageModel(List<ArticleDTO> articles)
    {
        Articles = articles;
    }

    public List<ArticleDTO> Articles { get; }
}