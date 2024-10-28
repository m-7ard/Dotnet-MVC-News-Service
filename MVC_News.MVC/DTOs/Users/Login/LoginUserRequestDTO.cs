namespace MVC_News.Application.DTOs.Users.Login;

public class LoginUserRequestDTO
{
    public LoginUserRequestDTO()
    {
    }

    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}