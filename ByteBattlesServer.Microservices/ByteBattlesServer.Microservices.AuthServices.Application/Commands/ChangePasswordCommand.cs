using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Commands;

public record ChangePasswordCommand(Guid userId, string OldPassword, string NewPassword) : IRequest<ChangePasswordResponseDto>;