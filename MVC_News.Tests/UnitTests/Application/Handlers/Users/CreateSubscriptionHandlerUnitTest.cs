using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Subscriptions;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Application.Validators.ValidSubscriptionDurationValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class CreateSubscriptionHandlerUnitTest
{
    private readonly User _freeUser_001;
    private readonly User _premiumUser_001;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserExistsValidator<UserId>> _mockUserExistsValidator;
    private readonly Mock<IValidSubscriptionDurationValidator<int>> _mockMalidSubscriptionDurationValidator;
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
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserId>>();
        _mockMalidSubscriptionDurationValidator = new Mock<IValidSubscriptionDurationValidator<int> >();

        _handler = new CreateSubscriptionHandler(
            userExistsValidatorAsync: _mockUserExistsValidator.Object,
            userRepository: _mockUserRepository.Object,
            validSubscriptionDurationValidator: _mockMalidSubscriptionDurationValidator.Object
        );
    }

    [Fact]
    public async Task CreateSubscription_ValidData_Success()
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: _freeUser_001.Id.Value, subscriptionDuration: 1);

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _freeUser_001.Id, _freeUser_001);
        SetupMockServices.SetupValidSubscriptionDurationValidatorSuccess(_mockMalidSubscriptionDurationValidator, 1, DateTime.MaxValue);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateSubscription_UserIsAlreadySubscribed_Failure()
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: _premiumUser_001.Id.Value, subscriptionDuration: 1);

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _premiumUser_001.Id, _premiumUser_001);
        SetupMockServices.SetupValidSubscriptionDurationValidatorSuccess(_mockMalidSubscriptionDurationValidator, 1, DateTime.MaxValue);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.StateMismatch, result.AsT1[0].Code);
    }

    [Fact]
    public async Task CreateSubscription_InvalidDuration_Failure()
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: _freeUser_001.Id.Value, subscriptionDuration: 100);

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _premiumUser_001.Id, _premiumUser_001);
        SetupMockServices.SetupValidSubscriptionDurationValidatorFailure(_mockMalidSubscriptionDurationValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }

    [Fact]
    public async Task CreateSubscription_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var command = new CreateSubscriptionCommand(userId: Guid.Empty, subscriptionDuration: 100);
        SetupMockServices.SetupUserExistsValidatorFailure(_mockUserExistsValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }
}