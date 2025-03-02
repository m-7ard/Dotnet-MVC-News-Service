using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Users.Register;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class RegisterUserHandlerUnitTest
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserExistsValidator<UserEmail>> _userExistsValidatorAsync;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerUnitTest()
    {
        // Dependencies
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _userExistsValidatorAsync = new Mock<IUserExistsValidator<UserEmail>>();

        _handler = new RegisterUserHandler(
            userRepository: _mockUserRepository.Object,
            passwordHasher: _mockPasswordHasher.Object,
            userExistsValidator: _userExistsValidatorAsync.Object
        );
    }

    [Fact]
    public async Task RegisterUser_ValidData_Success()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        var command = new RegisterUserCommand(
            email: mockUser.Email.Value,
            password: "password",
            displayName: mockUser.DisplayName
        );

        SetupMockServices.SetupUserExistsValidatorFailure(_userExistsValidatorAsync);

        _mockPasswordHasher
            .Setup(hasher => hasher.Hash(command.Password))
            .Returns(mockUser.PasswordHash);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
        _mockUserRepository.Verify(repo => repo.CreateAsync(It.Is<User>(d => d.PasswordHash == mockUser.PasswordHash)));
    }

    [Fact]
    public async Task RegisterUser_UserAlreadyExists_Failure()
    {
        // ARRANGE
        var mockUser = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        var command = new RegisterUserCommand(
            email: mockUser.Email.Value,
            password: "password",
            displayName: mockUser.DisplayName
        );

        SetupMockServices.SetupUserExistsValidatorSuccess(_userExistsValidatorAsync, mockUser.Email, mockUser);
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(mockUser.Email))
            .ReturnsAsync(mockUser);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelAlreadyExists, result.AsT1[0].Code);
    }
}