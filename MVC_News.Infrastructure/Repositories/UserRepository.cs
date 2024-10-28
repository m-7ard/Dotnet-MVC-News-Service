using Microsoft.EntityFrameworkCore;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using MVC_News.Infrastructure;
using MVC_News.Infrastructure.Mappers;

public class UserRepository : IUserRepository
{
    private readonly NewsDbContext _dbContext;

    public UserRepository(NewsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var entity = await _dbContext.User.SingleOrDefaultAsync(d => d.Email == email);
        return entity is null ? null : UserMapper.FromDbEntityToDomain(entity);
    }

    public async Task<User> CreateAsync(User user)
    {
        var dbEntity = UserMapper.FromDomainToDbEntity(user);
        _dbContext.Add(dbEntity);
        await _dbContext.SaveChangesAsync();
        return UserMapper.FromDbEntityToDomain(dbEntity);
    }
    public async Task UpdateAsync(User user)
    {
        var oldDbEntity = _dbContext.User.SingleAsync(d => d.Id == user.Id);
        var newDbEntity = UserMapper.FromDomainToDbEntity(user);
        _dbContext.Entry(oldDbEntity).CurrentValues.SetValues(newDbEntity);
        await _dbContext.SaveChangesAsync();
    }
}