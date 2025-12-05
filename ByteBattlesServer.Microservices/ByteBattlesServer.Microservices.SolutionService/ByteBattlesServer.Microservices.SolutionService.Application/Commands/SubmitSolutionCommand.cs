using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Commands;

public record SubmitSolutionCommand(
    Guid TaskId,
    Guid UserId,
    Guid LanguageId,
    string Code) : IRequest<SolutionDto>;