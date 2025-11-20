using ByteBattlesServer.Microservices.UserProfile.Application.Handlers;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public record SearchQueryParams(string? SearchTerm=null, int Page = 1, int PageSize = 10) : IRequest<List<StudentProfileDto>>;
