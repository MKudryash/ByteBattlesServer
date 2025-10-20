namespace ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

public class LanguageException : Exception
{
    public string ErrorCode { get; }

    public LanguageException(string message, string errorCode = "LANGUAGE_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}