using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record CreateLanguageCommand(
    string LanguageTitle,
    string LanguageShortTitle
    ): IRequest<LanguageDto>;