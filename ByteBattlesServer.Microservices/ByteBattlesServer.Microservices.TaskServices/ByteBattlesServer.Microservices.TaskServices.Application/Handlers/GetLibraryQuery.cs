using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public record GetLibraryQuery(Guid languageId):IRequest<List<LibraryDto>>;