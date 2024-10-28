namespace MVC_News.Application.DTOs.Users.Register;

public class RegisterUserRequestDTO
{
    public RegisterUserRequestDTO()
    {
    }

    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}