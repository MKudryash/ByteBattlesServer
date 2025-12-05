using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Queries;

public record GetRecentActivitiesQuery(Guid UserId, int Limit):IRequest<List<RecentActivityDto>>;