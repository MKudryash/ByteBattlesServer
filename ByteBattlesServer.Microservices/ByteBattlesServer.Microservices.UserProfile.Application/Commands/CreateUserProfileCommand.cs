using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Commands;

public record CreateUserProfileCommand(Guid UserId, string UserName, string email, bool isPublic, UserRole role) : IRequest<UserProfileDto>;
