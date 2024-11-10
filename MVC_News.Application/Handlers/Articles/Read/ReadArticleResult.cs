using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Articles.Read;

public class ReadArticleResult
{
    public ReadArticleResult(Article article)
    {
        Article = article;
    }

    public Article Article { get; set; }
}