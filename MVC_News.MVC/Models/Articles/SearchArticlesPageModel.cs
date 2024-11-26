using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Articles;

public class SearchArticlesPageModel : BaseViewModel
{
    public SearchArticlesPageModel(Guid? authorId, string? title, DateTime? createdAfter, DateTime? createdBefore, string? orderBy, int? limitBy, List<string>? tags, List<ArticleDTO> articles)
    {
        AuthorId = authorId;
        Title = title;
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
        OrderBy = orderBy;
        LimitBy = limitBy;
        Tags = tags;
        Articles = articles;
    }

    public List<ArticleDTO> Articles { get; }
    public Guid? AuthorId { get; } 
    public string? Title { get; } 
    public DateTime? CreatedAfter { get; } 
    public DateTime? CreatedBefore { get; }
    public string? OrderBy { get; }
    public int? LimitBy { get; }
    public List<string>? Tags { get; }
}