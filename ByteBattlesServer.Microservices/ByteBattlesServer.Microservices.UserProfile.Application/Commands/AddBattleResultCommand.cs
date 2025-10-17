using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Commands;

public record AddBattleResultCommand(
    Guid UserId,
    Guid BattleId,
    string OpponentName,
    BattleResultType Result,
    int ExperienceGained,
    int ProblemsSolved,
    TimeSpan CompletionTime) : IRequest;