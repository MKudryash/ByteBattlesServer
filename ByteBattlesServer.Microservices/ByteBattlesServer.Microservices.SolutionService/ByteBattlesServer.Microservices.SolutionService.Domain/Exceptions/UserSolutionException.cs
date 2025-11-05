namespace ByteBattlesServer.Microservices.SolutionService.Domain.Exceptions;

public class UserSolutionException : Exception
{
    public string ErrorCode { get; }

    public UserSolutionException(string message, string errorCode = "USER_SOLUTION_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}