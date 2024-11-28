using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Subscriptions;
using MVC_News.Application.Handlers.Users.Login;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Domain.Entities;
using MVC_News.Domain.Errors;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class CreateSubscriptionHandlerUnitTest
{
    private readonly User _freeUser_001;
    private readonly User _premiumUser_001;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateSubscriptionHandler _handler;
    private readonly DateTime _now;

    public CreateSubscriptionHandlerUnitTest()
    {
        // Users
        _freeUser_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        _premiumUser_001 = Mixins.CreateUser(seed: 2, isAdmin: false, subscriptions: []);
        _premiumUser_001.Subscriptions.Add(Mixins.CreatedValidSubscription(_premiumUser_001.Id));

        // Anchor date
        _now = DateTime.Now;

        // Dependencies
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new CreateSubscriptionHandler(
            userRepository: _mockUserRepository.Object
        );
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 6)]
    [InlineData(3, 12)]
    public async Task CreateSubscription_ValidData_Success(int duration, int increaseByMonths)
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: _freeUser_001.Id, subscriptionDuration: duration);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(_freeUser_001.Id))
            .ReturnsAsync(_freeUser_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
        Assert.NotNull(result.AsT0.User.GetActiveSubscription());
        Assert.True(result.AsT0.User.GetActiveSubscription()!.ExpirationDate > _now.AddMonths(increaseByMonths));
    }

    [Fact]
    public async Task CreateSubscription_UserIsAlreadySubscribed_Failure()
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: _premiumUser_001.Id, subscriptionDuration: 1);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(_premiumUser_001.Id))
            .ReturnsAsync(_premiumUser_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.DomainError, result.AsT1[0].Code);
        var metaData = (ApplicationDomainErrorMetadata)result.AsT1[0].Metadata;
        Assert.Equal(UserDomainErrorCodes.UserAlreadySubscribed, metaData.OriginalError.Code);
    }

    [Fact]
    public async Task CreateSubscription_InvalidDuration_Failure()
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: _freeUser_001.Id, subscriptionDuration: 100);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(_freeUser_001.Id))
            .ReturnsAsync(_freeUser_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.StateMismatch, result.AsT1[0].Code);
    }

    [Fact]
    public async Task CreateSubscription_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: Guid.Empty, subscriptionDuration: 100);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelDoesNotExist, result.AsT1[0].Code);
    }
}