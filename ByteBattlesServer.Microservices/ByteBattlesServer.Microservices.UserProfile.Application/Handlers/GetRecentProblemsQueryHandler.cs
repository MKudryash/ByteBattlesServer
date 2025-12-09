using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetRecentProblemsQueryHandler:IRequestHandler<GetRecentProblemsQuery,List<RecentProblemDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetRecentProblemsQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    public async Task<List<RecentProblemDto>> Handle(GetRecentProblemsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if(user == null)
            throw new UserProfileNotFoundException(request.UserId);
        var recentProblems = await _userProfileRepository.GetRecentProblemsAsync(user.Id);
       
        return  recentProblems.Select(x => new RecentProblemDto()
        {
            Title = x.Title,
            SolvedAt = x.SolvedAt,
            ProblemId = x.ProblemId,
            Difficulty = x.Difficulty.ToString(),
            Language = x.Language
        }).ToList();
    }
}