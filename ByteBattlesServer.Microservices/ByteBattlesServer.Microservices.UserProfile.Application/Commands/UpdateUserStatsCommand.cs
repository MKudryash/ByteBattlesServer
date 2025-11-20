using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Commands;

public record UpdateUserStatsCommand(
    Guid UserId,
    bool isSuccessful,
    TaskDifficulty difficulty,
    TimeSpan executionTime,
    Guid taskId,
    string problemTitle,
    string language
) : IRequest<UserProfileDto>;