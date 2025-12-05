using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record UpdateTaskCommand
(
    Guid TaskId,
    string Title,
    string Description ,
    string Difficulty ,
    string Author,
    string FunctionName,
    string PatternMain,
    string PatternFunction ,
    string Parameters,
    string ReturnType,
    List<Guid> LanguageIds,
    List<Guid> LibrariesIds,
    List<TestCaseDto> TestCases
    ): IRequest<TaskDto>;