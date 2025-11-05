namespace ByteBattlesServer.Microservices.SolutionService.Domain.Exceptions;

public class SolutionNotFoundException:UserSolutionException
{
    public SolutionNotFoundException(Guid solutionId) 
        : base($"Solution with ID {solutionId} was not found")
    {
    }
}