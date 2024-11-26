namespace MVC_News.MVC.DTOs.Contracts.Articles.Search;

public class SearchArticlesRequestDTO
{
    public SearchArticlesRequestDTO(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore, string? orderBy, int? limitBy, List<string>? tags, string? title)
    {
        AuthorId = authorId;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
        OrderBy = orderBy;
        LimitBy = limitBy;
        Tags = tags;
        Title = title;
    }

    public string? Title { get; }
    public Guid? AuthorId { get; }
    public DateTime? CreatedAfter { get; }
    public DateTime? CreatedBefore { get; }
    public string? OrderBy { get; }
    public int? LimitBy { get; }
    public List<string>? Tags { get; }
}