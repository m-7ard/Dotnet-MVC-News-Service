namespace MVC_News.Application.Errors;

public static class ApplicationValidatorErrorCodes
{
    public const string USER_WITH_ID_EXISTS_ERROR = "USER_WITH_ID_EXISTS_ERROR";
    public const string USER_WITH_EMAIL_EXISTS_ERROR = "USER_WITH_EMAIL_EXISTS_ERROR";
    public const string ARTICLE_EXISTS_ERROR = "ARTICLE_EXISTS_ERROR";
    public const string IS_VALID_SUBSCRIPTION_DURATION_ERROR = "IS_VALID_SUBSCRIPTION_DURATION_ERROR";
    public const string ARE_MATCHING_PASSWORDS_ERROR = "ARE_MATCHING_PASSWORDS_ERROR";
}