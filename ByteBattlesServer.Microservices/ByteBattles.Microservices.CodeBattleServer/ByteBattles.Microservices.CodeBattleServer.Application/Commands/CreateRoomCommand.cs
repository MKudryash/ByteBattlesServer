using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record CreateRoomCommand(string Name, Guid UserId, Guid LanguageId) : IRequest<ResponseCreateRoom>;