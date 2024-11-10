namespace MVC_News.MVC.DTOs.Contracts.Users.Login;

public class LoginUserRequestDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}