using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Commands;

public record CreateUserProfileCommand(Guid UserId, string UserName, bool isPublic) : IRequest<UserProfileDto>;
