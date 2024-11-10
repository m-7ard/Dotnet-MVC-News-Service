using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Articles.Update;

public class UpdateArticleResult
{
    public UpdateArticleResult(Article article)
    {
        Article = article;
    }

    public Article Article { get; }
}