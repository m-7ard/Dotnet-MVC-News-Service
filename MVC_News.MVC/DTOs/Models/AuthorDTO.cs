namespace MVC_News.MVC.DTOs.Models;

public class AuthorDTO
{
    public AuthorDTO(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public Guid Id { get; }
    public string DisplayName { get; }
}