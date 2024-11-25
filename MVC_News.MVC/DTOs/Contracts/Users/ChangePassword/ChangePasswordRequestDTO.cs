namespace MVC_News.MVC.DTOs.Contracts.Users.ChangePassword;

public class ChangePasswordRequestDTO
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}