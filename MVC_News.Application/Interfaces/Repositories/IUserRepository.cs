using MVC_News.Domain.Entities;

namespace MVC_News.Application.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User> CreateAsync(User user);
    public Task UpdateAsync(User user);
}