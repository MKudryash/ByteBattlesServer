using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Handlers;

public class SubmitUserStatsHandler: IRequestHandler<SubmitUserStatsCommand, SubmitUserStatsResponse>
{
    private readonly IMessageBus _messageBus;

    public SubmitUserStatsHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
    
    public Task<SubmitUserStatsResponse> Handle(SubmitUserStatsCommand request, CancellationToken cancellationToken)
    {
        var userUpdateStats = new UserStatsIntegrationEvent()
        {
            UserId = request.UserId,
            IsSuccessful = request.IsSuccessful,
            Difficulty = request.Task.Difficulty,
            ExecutionTime = request.ExecutionTime,
            TaskId = request.Task.Id,
            ProblemTitle = request.Task.Title?? "Задача решена",
            Language = request.Task.Language.Title,
            ActivityType = ActivityType.ProblemSolved
        };
        _messageBus.Publish(
            userUpdateStats,
            "user_stats-events",
            "user.stats.update");
        return Task.FromResult(new SubmitUserStatsResponse());
    }
}