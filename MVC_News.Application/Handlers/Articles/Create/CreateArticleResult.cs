namespace MVC_News.Application.Handlers.Articles.Create;

public class CreateArticleResult
{
    public CreateArticleResult(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}