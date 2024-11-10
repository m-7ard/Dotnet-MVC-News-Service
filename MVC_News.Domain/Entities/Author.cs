namespace MVC_News.Domain.Entities;

public class Author
{
    public Author(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public Guid Id { get; private set; }
    public string DisplayName { get; set; }
}