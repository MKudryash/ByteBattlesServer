using ByteBattlesServer.SharedContracts.IntegrationEvents;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record SubmitUserStatsCommand(Guid UserId, bool IsSuccessful, 
    TaskInfo Task,TimeSpan ExecutionTime,
    Guid BattleId, string name):IRequest<SubmitUserStatsResponse>;

public class SubmitUserStatsResponse(string message = "Success");