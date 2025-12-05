using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetUserActivityQueryHandler:IRequestHandler<GetUserActivityQuery,UserActivityResponse>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetUserActivityQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    public async Task<UserActivityResponse> Handle(GetUserActivityQuery request, CancellationToken cancellationToken)
    {
        var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if(user == null)
            throw new UserProfileNotFoundException(request.UserId);
        
        var recentProblems = await _userProfileRepository.GetRecentProblemsAsync(request.UserId);
        var recentActivities = await _userProfileRepository.GetRecentActivitiesAsync(request.UserId);

        return new UserActivityResponse()
        {
            RecentActivities = recentActivities.Select(x => new RecentActivityDto()
            {
                Title = x.Title,
                Description = x.Description,
                ExperienceGained = x.ExperienceGained,
                Timestamp = x.Timestamp,
                Type = x.Type.ToString()
            }).ToList(),
            RecentProblems = recentProblems.Select(x => new RecentProblemDto()
            {
                Title = x.Title,
                SolvedAt = x.SolvedAt,
                ProblemId = x.ProblemId,
                Difficulty = x.Difficulty.ToString(),
                Language = x.Language
            }).ToList()
        };
    }
}