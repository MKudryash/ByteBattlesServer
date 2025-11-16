using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record UpdateLibraryCommand(
    Guid Id,
    string Name,
    string Description,
    string Version) :
    IRequest<LibraryDto>;