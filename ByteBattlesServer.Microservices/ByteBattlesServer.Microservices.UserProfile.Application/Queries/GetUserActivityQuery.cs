using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Queries;

public record GetUserActivityQuery(Guid UserId, int? ActivitiesLimit, int? ProblemsLimit):IRequest<UserActivityResponse>;