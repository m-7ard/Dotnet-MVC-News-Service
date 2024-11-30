using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class DeleteArticlesPageModel : BaseViewModel
{
    public DeleteArticlesPageModel(ArticleDTO article)
    {
        Article = article;
    }

    public ArticleDTO Article { get; }
}