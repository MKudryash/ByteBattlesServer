using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Queries;

public record GetUserProfileByIdQuery(Guid ProfileId) : IRequest<UserProfileDto>;