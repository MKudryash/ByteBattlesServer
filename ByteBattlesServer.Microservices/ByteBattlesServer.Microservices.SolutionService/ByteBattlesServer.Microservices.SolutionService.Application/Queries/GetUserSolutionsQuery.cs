using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Queries;

public record GetUserSolutionsQuery(Guid UserId) : IRequest<List<SolutionDto>>;