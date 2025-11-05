using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record SearchLanguagesPagedQuery
(
    string? SearchTerm,
    int Page =  1,
    int PageSize = 20
):IRequest<List<LanguageDto>>;