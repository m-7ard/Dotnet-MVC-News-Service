using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class ManageArticlesPageModel : BaseViewModel
{
    public ManageArticlesPageModel(List<ArticleDTO> articles)
    {
        Articles = articles;
    }

    public List<ArticleDTO> Articles { get; }
}