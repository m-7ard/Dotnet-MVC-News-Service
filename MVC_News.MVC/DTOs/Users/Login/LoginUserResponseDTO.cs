using MVC_News.Domain.Entities;

namespace MVC_News.Application.DTOs.Users.Login;

public class LoginUserResponseDTO
{
    public LoginUserResponseDTO(User user)
    {
        User = user;
    }

    public User User { get; }
}