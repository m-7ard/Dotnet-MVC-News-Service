using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Users.Read;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class ReadUserHandlerUnitTest
{
    private readonly ReadUserHandler _handler;
    private readonly User _user_001;
    private readonly Mock<IUserExistsValidator<UserId>> _mockUserExistsValidator;
    
    public ReadUserHandlerUnitTest()
    {
        // Users
        _user_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);

        // Dependencies
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserId>>();

        _handler = new ReadUserHandler(
            userExistsValidator: _mockUserExistsValidator.Object
        );
    }

    [Fact]
    public async Task ReadUser_ValidData_Success()
    {
        // ARRANGE
        var command = new ReadUserQuery(id: _user_001.Id.Value);

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _user_001.Id, _user_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ReadUser_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var command = new ReadUserQuery(id: Guid.Empty);
        SetupMockServices.SetupUserExistsValidatorFailure(_mockUserExistsValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }
}