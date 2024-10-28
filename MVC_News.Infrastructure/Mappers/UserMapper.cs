using MVC_News.Domain.Entities;
using MVC_News.Infrastructure.DbEntities;

namespace MVC_News.Infrastructure.Mappers;

public static class UserMapper
{
    public static UserDbEntity FromDomainToDbEntity(User source)
    {
        return new UserDbEntity(
            id: source.Id,
            email: source.Email,
            passwordHash: source.PasswordHash,
            displayName: source.DisplayName,
            isAdmin: source.IsAdmin
        );
    }

    public static User FromDbEntityToDomain(UserDbEntity source)
    {
        return new User(
            id: source.Id,
            email: source.Email,
            passwordHash: source.PasswordHash,
            displayName: source.DisplayName,
            isAdmin: source.IsAdmin
        );
    }
}