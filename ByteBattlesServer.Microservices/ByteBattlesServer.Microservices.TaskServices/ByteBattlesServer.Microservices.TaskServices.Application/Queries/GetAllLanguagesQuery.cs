using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record GetAllLanguagesQuery
(
    string? SearchTerm,
    int Page,
    int PageSize
):IRequest<List<LanguageDto>>;