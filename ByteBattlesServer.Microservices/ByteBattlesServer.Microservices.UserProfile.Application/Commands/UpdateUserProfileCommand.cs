using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Commands;

public record UpdateUserProfileCommand(
    Guid UserId, 
    string UserName, 
    string? Bio, 
    string? Country, 
    string? GitHubUrl, 
    string? LinkedInUrl, 
    bool IsPublic) : IRequest<UserProfileDto>;