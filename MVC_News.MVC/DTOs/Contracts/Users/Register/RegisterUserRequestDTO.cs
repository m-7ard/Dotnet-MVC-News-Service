namespace MVC_News.MVC.DTOs.Contracts.Users.Register;

public class RegisterUserRequestDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}