namespace MVC_News.Application.Contracts.Criteria;


public class FilterAllArticlesCriteria
{
    public FilterAllArticlesCriteria(Guid? authorId, DateTime? createdAfter, DateTime? createdBefore, Tuple<string, bool>? orderBy, int? limitBy, List<string>? tags)
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
    public Tuple<string, bool>? OrderBy { get; set; }
    public int? LimitBy { get; set; }
    public List<string>? Tags { get; }
}