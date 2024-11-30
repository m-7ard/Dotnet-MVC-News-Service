namespace MVC_News.Application.Contracts.Criteria;


 public class FilterAllArticlesCriteria : IEquatable<FilterAllArticlesCriteria>
{
    public FilterAllArticlesCriteria(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore, Tuple<string, bool>? orderBy, int? limitBy, List<string>? tags, string? title)
    {
        AuthorId = authorId;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
        OrderBy = orderBy;
        LimitBy = limitBy;
        Tags = tags;
        Title = title;
    }

    public string? Title { get; set; }
    public Guid? AuthorId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public Tuple<string, bool>? OrderBy { get; set; }
    public int? LimitBy { get; set; }
    public List<string>? Tags { get; }

    // For Unit Testing
    public bool Equals(FilterAllArticlesCriteria? other)
    {
        if (other == null)
            return false;

        return AuthorId == other.AuthorId &&
                CreatedAfter == other.CreatedAfter &&
                CreatedBefore == other.CreatedBefore &&
                Equals(OrderBy, other.OrderBy) &&
                LimitBy == other.LimitBy &&
                Title == other.Title &&
                (Tags == other.Tags || (Tags != null && other.Tags != null && Tags.SequenceEqual(other.Tags)));
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FilterAllArticlesCriteria);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(AuthorId);
        hash.Add(CreatedAfter);
        hash.Add(CreatedBefore);
        hash.Add(OrderBy);
        hash.Add(LimitBy);
        hash.Add(Title);
        if (Tags != null)
        {
            foreach (var tag in Tags)
            {
                hash.Add(tag);
            }
        }
        return hash.ToHashCode();
    }
}