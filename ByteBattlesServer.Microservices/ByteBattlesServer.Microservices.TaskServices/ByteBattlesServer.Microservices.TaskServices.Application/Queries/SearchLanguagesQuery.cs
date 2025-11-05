using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record SearchLanguagesQuery
(
    string? SearchTerm
):IRequest<List<LanguageDto>>;