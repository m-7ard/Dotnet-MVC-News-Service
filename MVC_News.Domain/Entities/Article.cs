namespace MVC_News.Domain.Entities;

public class Article
{
    public Article(Guid id, string title, string content, DateTime dateCreated, string headerImage, Guid authorId, List<string> tags)
    {
        Id = id;
        Title = title;
        Content = content;
        DateCreated = dateCreated;
        HeaderImage = headerImage;
        AuthorId = authorId;
        Tags = tags;
    }

    public Guid Id { get; private set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string HeaderImage { get; set; }
    public DateTime DateCreated { get; set; }
    public Guid AuthorId { get; set; }
    public List<string> Tags { get; set; }
}