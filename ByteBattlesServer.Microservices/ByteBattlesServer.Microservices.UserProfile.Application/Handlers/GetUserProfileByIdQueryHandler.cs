// Application/Queries/GetUserProfileByIdQueryHandler.cs

using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetUserProfileByIdQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        // Находим профиль по ID профиля (не UserId)
        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.ProfileId);
        
        if (userProfile == null)
        {
            throw new UserProfileNotFoundException(request.ProfileId);
        }

     
        return MapToUserProfileDto(userProfile);
    }
    private static UserProfileDto MapToUserProfileDto(Domain.Entities.UserProfile userProfile)
    {
        return new UserProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            UserName = userProfile.UserName,
            Bio = userProfile.Bio,
            Country = userProfile.Country,
            GitHubUrl = userProfile.GitHubUrl,
            LinkedInUrl = userProfile.LinkedInUrl,
            IsPublic = userProfile.IsPublic,
            CreatedAt = userProfile.CreatedAt,

        };
    }
}