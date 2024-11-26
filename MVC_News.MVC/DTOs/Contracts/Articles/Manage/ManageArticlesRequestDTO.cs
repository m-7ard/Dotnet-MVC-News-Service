namespace MVC_News.MVC.DTOs.Contracts.Articles.Manage;

public class ManageArticlesRequestDTO
{
    public ManageArticlesRequestDTO(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore, string? orderBy, int? limitBy, List<string>? tags, string? title)
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
    public string? OrderBy { get; }
    public int? LimitBy { get; }
    public List<string>? Tags { get; }
}