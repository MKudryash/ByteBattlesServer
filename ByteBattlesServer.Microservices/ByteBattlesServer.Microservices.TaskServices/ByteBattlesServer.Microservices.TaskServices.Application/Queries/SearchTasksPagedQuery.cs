using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record SearchTasksPagedQuery
(
    string? SearchTerm,
    TaskDifficulty? Difficulty,
     Guid? LanguageId,
     int Page =  1,
     int PageSize = 20
):IRequest<List<TaskDto>>;