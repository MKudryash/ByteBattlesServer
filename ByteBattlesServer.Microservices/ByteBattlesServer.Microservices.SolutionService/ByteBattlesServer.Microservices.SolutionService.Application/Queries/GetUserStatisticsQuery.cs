using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Queries;

public record GetUserStatisticsQuery(Guid UserId) : IRequest<SolutionStatisticsDto>;