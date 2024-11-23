using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class FrontpagePageModel : BaseViewModel
{
    public FrontpagePageModel(List<ArticleDTO> mainArticles, List<ArticleDTO> newestArticles)
    {
        MainArticles = mainArticles;
        NewestArticles = newestArticles;
    }

    public List<ArticleDTO> MainArticles { get; }
    public List<ArticleDTO> NewestArticles { get; }
}