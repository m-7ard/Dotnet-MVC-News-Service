using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Users.ChangePassword;
using MVC_News.Application.Handlers.Users.Login;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class LoginUserHandlerUnitTest
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerUnitTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _handler = new LoginUserHandler(
            userRepository: _mockUserRepository.Object,
            passwordHasher: _mockPasswordHasher.Object
        );
    }

    [Fact]
    public async Task LoginUser_ValidData_Success()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false);
        var command = new LoginUserQuery(
            email: mockUser.Email,
            password: "password"
        );

        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(mockUser.Email))
            .ReturnsAsync(mockUser);

        _mockPasswordHasher
            .Setup(hasher => hasher.Verify(mockUser.PasswordHash, command.Password))
            .Returns(true);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task LoginUser_UserDoeNotExist_Failure()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false);
        var command = new LoginUserQuery(
            email: mockUser.Email,
            password: "password"
        );

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.Custom, result.AsT1[0].Code);
        _mockUserRepository.Verify(d => d.GetUserByEmailAsync(command.Email), Times.Once);
    }

    [Fact]
    public async Task LoginUser_InvalidPassword_Failure()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false);
        var command = new LoginUserQuery(
            email: mockUser.Email,
            password: "password"
        );
        
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(mockUser.Email))
            .ReturnsAsync(mockUser);

        _mockPasswordHasher
            .Setup(hasher => hasher.Verify(mockUser.PasswordHash, command.Password))
            .Returns(false);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.Custom, result.AsT1[0].Code);
        _mockPasswordHasher.Verify(d => d.Verify(mockUser.PasswordHash, command.Password), Times.Once);
    }
}