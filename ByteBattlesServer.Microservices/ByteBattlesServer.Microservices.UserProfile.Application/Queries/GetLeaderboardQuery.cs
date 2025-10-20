using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Queries;

public record GetLeaderboardQuery (int topCount=5): IRequest<List<LeaderboardUserDto>>;