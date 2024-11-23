using Microsoft.EntityFrameworkCore;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using MVC_News.Infrastructure.Mappers;

namespace MVC_News.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NewsDbContext _dbContext;

    public UserRepository(NewsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var entity = await _dbContext.User
            .Include(d => d.Subscriptions)
            .SingleOrDefaultAsync(d => d.Email == email);
        return entity is null ? null : UserMapper.FromDbEntityToDomain(entity);
    }

    public async Task<User> CreateAsync(User user)
    {
        var dbEntity = UserMapper.FromDomainToDbEntity(user);
        _dbContext.Add(dbEntity);
        /* Maybe: Persist the subscriptions over here also */
        await _dbContext.SaveChangesAsync();
        return UserMapper.FromDbEntityToDomain(dbEntity);
    }
    public async Task UpdateAsync(User user)
    {
        var oldDbEntity = await _dbContext.User.Include(d => d.Subscriptions).SingleAsync(d => d.Id == user.Id);
        var newDbEntity = UserMapper.FromDomainToDbEntity(user);
        _dbContext.Entry(oldDbEntity).CurrentValues.SetValues(newDbEntity);

        var oldSubs = oldDbEntity.Subscriptions.ToDictionary(item => item.Id);
        var updatedSubs = newDbEntity.Subscriptions.ToDictionary(item => item.Id);

        foreach (var (id, oldSub) in oldSubs)
        {
            if (!updatedSubs.TryGetValue(id, out var updatedSub))
            {
                _dbContext.Subscription.Remove(oldSub);
            }
            else
            {
                _dbContext.Entry(oldSub).CurrentValues.SetValues(newDbEntity);
            }
        }

        foreach (var (id, updatedSub) in updatedSubs)
        {
            if (!oldSubs.TryGetValue(id, out var newSub))
            {
                _dbContext.Subscription.Add(updatedSub);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserById(Guid id)
    {
        var entity = await _dbContext.User
            .Include(d => d.Subscriptions)
            .SingleOrDefaultAsync(d => d.Id == id);
        return entity is null ? null : UserMapper.FromDbEntityToDomain(entity);
    }
}
