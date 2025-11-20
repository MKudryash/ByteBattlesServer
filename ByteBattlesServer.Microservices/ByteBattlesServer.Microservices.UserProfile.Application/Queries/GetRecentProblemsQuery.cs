using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Queries;

public record GetRecentProblemsQuery(Guid UserId, int Limit):IRequest<List<RecentProblemDto>>;