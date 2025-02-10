using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Validators.ArticleExistsValidator;
using MVC_News.Application.Validators.MatchingPasswordHashValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Application.Validators.ValidSubscriptionDurationValidator;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Tests.UnitTests.Utils;

public static class SetupMockServices
{
    // IUserExistsValidator
    public static void SetupUserExistsValidatorSuccess<T>(Mock<IUserExistsValidator<T>> mockValidator, T input, User output)
    {
        mockValidator
            .Setup(validator => validator.Validate(It.Is<T>(id => Equals(id, input))))
            .ReturnsAsync(OneOf<User, List<ApplicationError>>.FromT0(output));
    }

    public static void SetupUserExistsValidatorFailure<T>(Mock<IUserExistsValidator<T>> mockValidator)
    {
        mockValidator
            .Setup(validator => validator.Validate(It.IsAny<T>()))
            .ReturnsAsync(OneOf<User, List<ApplicationError>>.FromT1([EmptyApplicationError.Instance]));
    }

    // IArticleExistsValidator
    public static void SetupArticleExistsValidatorSuccess<T>(Mock<IArticleExistsValidator<T>> mockValidator, T input, Article output)
    {
        mockValidator
            .Setup(validator => validator.Validate(It.Is<T>(id => Equals(id, input))))
            .ReturnsAsync(OneOf<Article, List<ApplicationError>>.FromT0(output));
    }

    public static void SetupArticleExistsValidatorFailure<T>(Mock<IArticleExistsValidator<T>> mockValidator)
    {
        mockValidator
            .Setup(validator => validator.Validate(It.IsAny<T>()))
            .ReturnsAsync(OneOf<Article, List<ApplicationError>>.FromT1([EmptyApplicationError.Instance]));
    }

    // IMatchingPasswordHashValidator
    public static void SetupMatchingPasswordHashValidatorSuccess<T>(Mock<IMatchingPasswordHashValidator<T>> mockValidator, T hashedPassword, T plainPassword, bool output)
    {
        mockValidator
            .Setup(validator => validator.Validate(hashedPassword, plainPassword))
            .Returns(OneOf<bool, List<ApplicationError>>.FromT0(output));
    }

    public static void SetupMatchingPasswordHashValidatorFailure<T>(Mock<IMatchingPasswordHashValidator<T>> mockValidator)
    {
        mockValidator
            .Setup(validator => validator.Validate(It.IsAny<T>(), It.IsAny<T>()))
            .Returns(OneOf<bool, List<ApplicationError>>.FromT1([EmptyApplicationError.Instance]));
    }

    // IValidSubscriptionDurationValidator
    public static void SetupValidSubscriptionDurationValidatorSuccess<T>(Mock<IValidSubscriptionDurationValidator<T>> mockValidator, T input, DateTime output)
    {
        mockValidator
            .Setup(validator => validator.Validate(input))
            .Returns(OneOf<DateTime, List<ApplicationError>>.FromT0(output));
    }

    public static void SetupValidSubscriptionDurationValidatorFailure<T>(Mock<IValidSubscriptionDurationValidator<T>> mockValidator)
    {
        mockValidator
            .Setup(validator => validator.Validate(It.IsAny<T>()))
            .Returns(OneOf<DateTime, List<ApplicationError>>.FromT1([EmptyApplicationError.Instance]));
    }
}