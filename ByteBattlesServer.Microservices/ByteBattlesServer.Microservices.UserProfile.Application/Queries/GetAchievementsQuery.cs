using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Queries;

public record GetAchievementsQuery(Guid UserId):IRequest<List<AchievementDto>>;