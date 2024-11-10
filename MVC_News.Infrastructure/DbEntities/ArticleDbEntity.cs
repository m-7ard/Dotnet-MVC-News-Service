namespace MVC_News.Infrastructure.DbEntities;

public class ArticleDbEntity
{
    public ArticleDbEntity(Guid id, string title, string content, DateTime dateCreated, Guid authorId, string headerImage)
    {
        Id = id;
        Title = title;
        Content = content;
        DateCreated = dateCreated;
        AuthorId = authorId;
        HeaderImage = headerImage;
    }

    public Guid Id { get; private set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string HeaderImage { get; set; }
    public DateTime DateCreated { get; set; }
    
    // User FK
    public Guid AuthorId { get; set; }
    public UserDbEntity Author { get; set; } = null!; 
}