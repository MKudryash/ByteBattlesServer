using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record GetLanguageByIdQuery(Guid LanguageId) :IRequest<LanguageDto?>;