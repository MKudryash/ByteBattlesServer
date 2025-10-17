using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Commands;

public record CreateUserProfileCommand(Guid UserId, string UserName) : IRequest<UserProfileDto>;


// // UserProfile.Application/Queries/GetLeaderboardQuery.cs
// public record GetLeaderboardQuery(int Page, int PageSize) : IRequest<LeaderboardDto>;
//
// // UserProfile.Application/Queries/SearchUsersQuery.cs
// public record SearchUsersQuery(string SearchTerm, int Page, int PageSize) : IRequest<Result<SearchResultsDto>>;