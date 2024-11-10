namespace MVC_News.MVC.DTOs.Models;

public class ArticleDTO
{
    public ArticleDTO(Guid id, string title, string content, string headerImage, DateTime dateCreated, AuthorDTO author)
    {
        Id = id;
        Title = title;
        Content = content;
        HeaderImage = headerImage;
        DateCreated = dateCreated;
        Author = author;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Content { get; }
    public string HeaderImage { get; }
    public DateTime DateCreated { get; }
    public AuthorDTO Author { get; }
}