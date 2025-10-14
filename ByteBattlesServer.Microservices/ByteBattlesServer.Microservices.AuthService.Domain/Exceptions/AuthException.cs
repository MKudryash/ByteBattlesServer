namespace ByteBattlesServer.Microservices.AuthService.Domain.Exceptions;


public class AuthException : Exception
{
    public string ErrorCode { get; }

    public AuthException(string message, string errorCode = "AUTH_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}

// AuthService.Domain/Exceptions/ValidationException.cs
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Validation failed")
    {
        Errors = errors;
    }

    public ValidationException(string property, string error) 
        : this(new Dictionary<string, string[]> { [property] = new[] { error } })
    {
    }
}