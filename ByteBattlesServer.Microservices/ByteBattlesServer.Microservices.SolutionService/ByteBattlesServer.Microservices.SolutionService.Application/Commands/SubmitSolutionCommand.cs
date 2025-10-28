using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Commands;

public record SubmitSolutionCommand(
    Guid TaskId,
    Guid UserId,
    Guid LanguageId,
    string Code) : IRequest<SolutionDto>;