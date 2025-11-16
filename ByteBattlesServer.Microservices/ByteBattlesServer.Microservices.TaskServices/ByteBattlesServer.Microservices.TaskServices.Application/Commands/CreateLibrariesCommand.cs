using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record CreateLibrariesCommand(
    Guid LanguageId,
    List<CreateLibraryDto> Libraries
) : IRequest<List<LibraryDto>>;