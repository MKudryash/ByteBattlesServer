using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetRecentActivitiesQueryHandler:IRequestHandler<GetRecentActivitiesQuery,List<RecentActivityDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetRecentActivitiesQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    
    public async Task<List<RecentActivityDto>> Handle(GetRecentActivitiesQuery request, CancellationToken cancellationToken)
    {
       var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
       if(user == null)
           throw new UserProfileNotFoundException(request.UserId);
       
       var recentActivities = await _userProfileRepository.GetRecentActivitiesAsync(user.Id);
       return recentActivities.Select(x=>new RecentActivityDto()
       {
           Title = x.Title,
           Description = x.Description,
           ExperienceGained =  x.ExperienceGained,
           Timestamp = x.Timestamp,
           Type = x.Type.ToString()
       }).ToList();
    }
}