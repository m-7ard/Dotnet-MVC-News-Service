using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Users.Login;
using MVC_News.Application.Handlers.Users.Read;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class ReadUserHandlerUnitTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ReadUserHandler _handler;
    private readonly User _user_001;

    public ReadUserHandlerUnitTest()
    {
        // Users
        _user_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);

        // Dependencies
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new ReadUserHandler(
            userRepository: _mockUserRepository.Object
        );
    }

    [Fact]
    public async Task ReadUser_ValidData_Success()
    {
        // ARRANGE
        var command = new ReadUserQuery(id: _user_001.Id);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(_user_001.Id))
            .ReturnsAsync(_user_001);

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

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelDoesNotExist, result.AsT1[0].Code);
    }
}