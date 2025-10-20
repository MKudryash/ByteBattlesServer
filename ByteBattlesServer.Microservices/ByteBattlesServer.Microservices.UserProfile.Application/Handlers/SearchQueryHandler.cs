using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class SearchQueryHandler:IRequestHandler<SearchQueryParams,List<UserProfileDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public SearchQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    public async Task<List<UserProfileDto>> Handle(SearchQueryParams request, CancellationToken cancellationToken)
    {
        var searchUsers = await _userProfileRepository.SearchAsync(request.SearchTerm, request.Page, request.PageSize);
        return await Task.FromResult(searchUsers.Select((profile) => MapToUserProfileDto(profile)).ToList());
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