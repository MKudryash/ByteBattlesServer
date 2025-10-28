namespace ByteBattlesServer.Microservices.SolutionService.Domain.Exceptions;

public class SolutionAccessDeniedException : UserSolutionException
{
    public SolutionAccessDeniedException(Guid solutionId, Guid userId) 
        : base($"User {userId} does not have access to solution {solutionId}")
    {
    }
}