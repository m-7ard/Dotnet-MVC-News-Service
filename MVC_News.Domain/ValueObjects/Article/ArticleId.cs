using OneOf;

namespace MVC_News.Domain.ValueObjects.Article;

public class ArticleId : ValueObject
{
    private ArticleId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static OneOf<bool, string> CanCreate(Guid value)
    {
        return true;
    }

    public static ArticleId ExecuteCreate(Guid value)
    {
        return new ArticleId(value);
    }

    public static ArticleId NewArticleId()
    {
        return ExecuteCreate(Guid.NewGuid());
    }


    public static OneOf<ArticleId, string> TryCreate(Guid value)
    {
        var canCreateResult = CanCreate(value);
        if (canCreateResult.TryPickT1(out var error, out _))
        {
            return error;
        }

        return ExecuteCreate(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public override string ToString()
    {
        return Value.ToString();
    }
}