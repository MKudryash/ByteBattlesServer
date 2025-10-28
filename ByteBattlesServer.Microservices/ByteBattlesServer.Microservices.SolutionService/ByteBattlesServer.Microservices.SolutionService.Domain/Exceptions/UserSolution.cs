namespace ByteBattlesServer.Microservices.SolutionService.Domain.Exceptions;

public class UserSolution : Exception
{
    public string ErrorCode { get; }

    public UserSolution(string message, string errorCode = "USER_SOLUTION_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}