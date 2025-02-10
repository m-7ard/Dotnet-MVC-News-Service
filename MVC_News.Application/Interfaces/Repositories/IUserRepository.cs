using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;

namespace MVC_News.Application.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<User?> GetUserByEmailAsync(UserEmail email);
    public Task<User?> GetUserById(UserId id);
    public Task CreateAsync(User user);
    public Task UpdateAsync(User user);
}