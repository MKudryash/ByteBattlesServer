using MediatR;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Commands;

public record RevokeTokenCommand(string RefreshToken, string IpAddress) : IRequest;