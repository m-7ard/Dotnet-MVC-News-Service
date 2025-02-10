using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Subscription;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Infrastructure.DbEntities;

namespace MVC_News.Infrastructure.Mappers;

public static class SubscriptionMapper
{
    public static SubscriptionDbEntity FromDomainToDbEntity(Subscription source)
    {
        return new SubscriptionDbEntity(
            id: source.Id,
            userId: source.UserId.Value,
            startDate: source.Dates.StartDate,
            expirationDate: source.Dates.ExpirationDate
        );
    }

    public static Subscription FromDbEntityToDomain(SubscriptionDbEntity source)
    {
        var subscriptionDates = SubscriptionDates.ExecuteCreate(startDate: source.StartDate, expirationDate: source.ExpirationDate);

        return new Subscription(
            id: source.Id,
            userId: UserId.ExecuteCreate(source.UserId),
            subscriptionDates: subscriptionDates
        );
    }
}