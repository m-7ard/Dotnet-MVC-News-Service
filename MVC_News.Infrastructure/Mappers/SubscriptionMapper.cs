using MVC_News.Domain.Entities;
using MVC_News.Infrastructure.DbEntities;

namespace MVC_News.Infrastructure.Mappers;

public static class SubscriptionMapper
{
    public static SubscriptionDbEntity FromDomainToDbEntity(Subscription source)
    {
        return new SubscriptionDbEntity(
            id: source.Id,
            userId: source.UserId,
            startDate: source.StartDate,
            expirationDate: source.ExpirationDate
        );
    }

    public static Subscription FromDbEntityToDomain(SubscriptionDbEntity source)
    {
        return new Subscription(
            id: source.Id,
            userId: source.UserId,
            startDate: source.StartDate,
            expirationDate: source.ExpirationDate
        );
    }
}