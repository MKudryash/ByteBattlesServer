using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken, string IpAddress) 
    : IRequest<AuthResponseDto>;