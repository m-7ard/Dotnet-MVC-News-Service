namespace MVC_News.MVC.DTOs.Contracts.Articles.List;

public class ListArticlesRequestDTO
{
    public ListArticlesRequestDTO(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore)
    {
        AuthorId = authorId;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
    }

    public Guid? AuthorId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}