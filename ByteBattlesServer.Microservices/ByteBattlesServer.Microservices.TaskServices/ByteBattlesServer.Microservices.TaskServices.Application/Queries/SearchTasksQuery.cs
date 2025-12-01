using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record SearchTasksQuery
(
    string? SearchTerm,
    TaskDifficulty? Difficulty,
    Guid? LanguageId
):IRequest<List<TaskDto>>;