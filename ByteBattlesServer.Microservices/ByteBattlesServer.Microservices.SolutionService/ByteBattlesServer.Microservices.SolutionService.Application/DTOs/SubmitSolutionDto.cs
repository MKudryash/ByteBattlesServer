using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.SolutionService.Application.DTOs;


public record SubmitSolutionDto(
    Guid TaskId,
    Guid LanguageId,
    string Code);