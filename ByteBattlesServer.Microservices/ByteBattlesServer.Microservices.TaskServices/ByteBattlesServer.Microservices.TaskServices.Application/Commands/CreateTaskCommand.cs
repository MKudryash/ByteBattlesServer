using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record CreateTaskCommand (
string Title,
string Description ,
string Difficulty ,
string Author,
string FunctionName,
string PatternMain,
string PatternFunction ,
List<Guid> LanguageIds):IRequest<TaskDto>;