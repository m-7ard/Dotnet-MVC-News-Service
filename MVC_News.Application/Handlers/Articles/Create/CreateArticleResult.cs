using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Articles.Create;

public class CreateArticleResult
{
    public CreateArticleResult(Article article)
    {
        Article = article;
    }

    public Article Article { get; }
}