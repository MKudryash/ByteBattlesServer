using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetAchievementsQueryHandler:IRequestHandler<GetAchievementsQuery,List<AchievementDto>>
{
    
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IAchievementRepository _achievementRepository;

    public GetAchievementsQueryHandler(IUserProfileRepository userProfileRepository, IAchievementRepository achievementRepository)
    {
        _userProfileRepository = userProfileRepository;
        _achievementRepository = achievementRepository;
    }

    
    public async Task<List<AchievementDto>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
    {
        // Находим профиль по ID профиля (не UserId)
        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        
        if (userProfile == null)
        {
            throw new UserProfileNotFoundException(request.UserId);
        }
        var achievements = userProfile.Achievements;
        
        return achievements.Select(x=>MapToDto(x.Achievement, x.Progress,x.UnlockedAt)).ToList();
    }

    private AchievementDto MapToDto(Achievement achievement,int progress, DateTime? unlockedAt) => new()
    {
        Id = achievement.Id,
        Description = achievement.Description,
        Name = achievement.Name,
        IconUrl = achievement.IconUrl,
        Category = achievement.Category.ToString(),
        Rarity = achievement.Rarity.ToString(),
        IsSecret = achievement.IsSecret,
        UnlockedAt = unlockedAt,
        Progress = progress
    };
}