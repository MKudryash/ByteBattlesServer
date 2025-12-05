using ByteBattlesServer.Domain.enums;

namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public record CreateUserProfileCommandDto(Guid UserId, string UserName, bool IsPublic, string Email, UserRole Role);
