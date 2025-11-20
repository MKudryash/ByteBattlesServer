using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Queries;

public record GetUserProfileQuery(Guid UserId, UserRole Role) : IRequest<UserProfileDto>;