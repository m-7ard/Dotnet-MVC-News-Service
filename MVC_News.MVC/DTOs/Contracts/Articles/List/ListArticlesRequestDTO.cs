namespace MVC_News.MVC.DTOs.Contracts.Articles.List;

public class ListArticlesRequestDTO
{
    public ListArticlesRequestDTO(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore, string? orderBy, int? limitBy, List<string>? tags)
    {
        AuthorId = authorId;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
        OrderBy = orderBy;
        LimitBy = limitBy;
        Tags = tags;
    }

    public Guid? AuthorId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string? OrderBy { get; }
    public int? LimitBy { get; }
    public List<string>? Tags { get; }
}