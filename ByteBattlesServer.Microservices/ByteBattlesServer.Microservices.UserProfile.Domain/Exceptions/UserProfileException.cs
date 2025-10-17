namespace ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;


public class UserProfileException : Exception
{
    public string ErrorCode { get; }

    public UserProfileException(string message, string errorCode = "USER_PROFILE_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
