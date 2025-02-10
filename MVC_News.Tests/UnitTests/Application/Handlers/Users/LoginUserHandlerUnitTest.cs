using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Users.Login;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators.MatchingPasswordHashValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class LoginUserHandlerUnitTest
{
    private readonly Mock<IUserExistsValidator<UserEmail>> _mockUserExistsValidator;
    private readonly Mock<IMatchingPasswordHashValidator<string>> _mockMatchingPasswordHashValidator;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerUnitTest()
    {
        // Dependencies
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserEmail>>();
        _mockMatchingPasswordHashValidator = new Mock<IMatchingPasswordHashValidator<string>>();

        _handler = new LoginUserHandler(
            userExistsValidator: _mockUserExistsValidator.Object,
            areMatchingPasswordsValidator: _mockMatchingPasswordHashValidator.Object
        );
    }

    [Fact]
    public async Task LoginUser_ValidData_Success()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        var command = new LoginUserQuery(
            email: mockUser.Email.Value,
            password: "password"
        );

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, mockUser.Email, mockUser);
        SetupMockServices.SetupMatchingPasswordHashValidatorSuccess(_mockMatchingPasswordHashValidator, mockUser.PasswordHash, command.Password, true);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task LoginUser_UserDoeNotExist_Failure()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        var command = new LoginUserQuery(
            email: mockUser.Email.Value,
            password: "password"
        );
        SetupMockServices.SetupUserExistsValidatorFailure(_mockUserExistsValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.Custom, result.AsT1[0].Code);
    }

    [Fact]
    public async Task LoginUser_InvalidPassword_Failure()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        var command = new LoginUserQuery(
            email: mockUser.Email.Value,
            password: "password"
        );
                
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, mockUser.Email, mockUser);
        SetupMockServices.SetupMatchingPasswordHashValidatorFailure(_mockMatchingPasswordHashValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.Custom, result.AsT1[0].Code);
    }
}