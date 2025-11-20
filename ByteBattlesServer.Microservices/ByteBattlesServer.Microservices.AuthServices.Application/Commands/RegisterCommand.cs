using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Commands;


public record RegisterCommand(string Email, string Password, string FirstName, string LastName, UserRole Role) 
    : IRequest<AuthResponseDto>;