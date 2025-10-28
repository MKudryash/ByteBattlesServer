using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Queries;

public record GetSolutionQuery(Guid SolutionId) : IRequest<SolutionDto>;
public record GetUserSolutionsQuery(Guid UserId) : IRequest<List<SolutionDto>>;
public record GetTaskSolutionsQuery(Guid TaskId, Guid UserId) : IRequest<List<SolutionDto>>;
public record GetUserStatisticsQuery(Guid UserId) : IRequest<SolutionStatisticsDto>;