using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Queries;

public record GetTaskSolutionsQuery(Guid TaskId, Guid UserId) : IRequest<List<SolutionDto>>;