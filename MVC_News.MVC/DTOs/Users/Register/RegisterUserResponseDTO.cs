using MVC_News.Domain.Entities;

namespace MVC_News.Application.DTOs.Users.Register;

public class RegisterUserResponseDTO
{
    public RegisterUserResponseDTO(User user)
    {
        User = user;
    }

    public User User { get; }
}