using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Users.ChangePassword;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators.MatchingPasswordHashValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Users;

public class ChangePasswordHandlerUnitTest
{
    private readonly User _freeUser_001;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IUserExistsValidator<UserId>> _mockUserExistsValidator;
    private readonly Mock<IMatchingPasswordHashValidator<string>> _mockMatchingPasswordHashValidator;
    private readonly ChangePasswordHandler _handler;

    public ChangePasswordHandlerUnitTest()
    {
        // Users
        _freeUser_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);

        // Dependencies
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserId>>();
        _mockMatchingPasswordHashValidator = new Mock<IMatchingPasswordHashValidator<string>>();

        _handler = new ChangePasswordHandler(
            userRepository: _mockUserRepository.Object,
            passwordHasher: _mockPasswordHasher.Object,
            userExistsValidator: _mockUserExistsValidator.Object,
            areMatchingPasswordsValidator: _mockMatchingPasswordHashValidator.Object
        );
    }

    [Fact]
    public async Task ChangePassword_ValidData_Success()
    {
        var newPassword = "new_password";
        var currentPassword = "current_password";

        // ARRANGE
        var command = new ChangePasswordCommand(
            id: _freeUser_001.Id.Value,
            currentPassword: currentPassword,
            newPassword: newPassword
        );

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _freeUser_001.Id, _freeUser_001);
        SetupMockServices.SetupMatchingPasswordHashValidatorSuccess(_mockMatchingPasswordHashValidator, _freeUser_001.PasswordHash, currentPassword, true);

        _mockPasswordHasher
            .Setup(hasher => hasher.Hash(newPassword))
            .Returns("hashed_new_password");

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
        Assert.Equal("hashed_new_password", result.AsT0.User.PasswordHash);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.Is<User>(user => user.PasswordHash == "hashed_new_password")));
    }

    [Fact]
    public async Task ChangePassword_WrongCurrentPassword_Failure()
    {
        var newPassword = "new_password";
        var currentPassword = "not_current_password";

        // ARRANGE
        var command = new ChangePasswordCommand(
            id: _freeUser_001.Id.Value,
            currentPassword: currentPassword,
            newPassword: newPassword
        );


        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _freeUser_001.Id, _freeUser_001);
        SetupMockServices.SetupMatchingPasswordHashValidatorFailure(_mockMatchingPasswordHashValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }
}