namespace MVC_News.MVC.DTOs.Models;

public class AuthorDTO
{
    public static readonly AuthorDTO UNKOWN_AUTHOR = new AuthorDTO(Guid.Empty, "Unkown Author"); 

    public AuthorDTO(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public Guid Id { get; }
    public string DisplayName { get; }
}