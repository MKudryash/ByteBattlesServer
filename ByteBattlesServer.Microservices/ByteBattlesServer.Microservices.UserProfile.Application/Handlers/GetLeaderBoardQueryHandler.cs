using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetLeaderBoardQueryHandler : IRequestHandler<GetLeaderboardQuery, List<LeaderboardUserDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetLeaderBoardQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    public async Task<List<LeaderboardUserDto>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var leaderboard = await _userProfileRepository.GetLeaderboardAsync(request.topCount);
        
       return await Task.FromResult(leaderboard.Select((profile, index) => MapToLeaderboardDto(profile, index + 1)).ToList());
    }

    private static LeaderboardUserDto MapToLeaderboardDto(Domain.Entities.UserProfile userProfile, int position)
    {
        return new LeaderboardUserDto
        {
            UserId = userProfile.UserId,
            UserName = userProfile.UserName,
            AvatarUrl = userProfile.AvatarUrl,
            Country = userProfile.Country,
            Position = position,
            TotalExperience = userProfile.Stats?.TotalExperience ?? 0,
            BattlesWon = userProfile.Stats?.Wins?? 0,
            ProblemsSolved = userProfile.Stats?.TotalProblemsSolved??  0,
            Level = userProfile.Level.ToString(),
        };
    }
}

