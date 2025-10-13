using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Commands;

public record LoginCommand(string Email, string Password, string IpAddress) : IRequest<AuthResponseDto>;