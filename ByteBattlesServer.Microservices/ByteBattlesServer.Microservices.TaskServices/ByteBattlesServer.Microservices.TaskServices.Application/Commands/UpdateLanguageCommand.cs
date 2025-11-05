using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record UpdateLanguageCommand(
    Guid LanguageId,
    string LanguageTitle,
    string LanguageShortTitle,
    string FileExtension ,
    string CompilerCommand,
    string ExecutionCommand,
    bool SupportsCompilation):IRequest<LanguageDto>;