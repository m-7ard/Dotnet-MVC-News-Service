using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Articles.List;

public class ListArticlesResult
{
    public ListArticlesResult(List<Article> articles)
    {
        Articles = articles;
    }

    public List<Article> Articles { get; set; }
}