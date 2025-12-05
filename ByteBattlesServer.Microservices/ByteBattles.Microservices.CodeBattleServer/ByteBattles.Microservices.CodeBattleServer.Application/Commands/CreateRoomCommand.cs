using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record CreateRoomCommand(string Name, Guid UserId, Guid LanguageId, TaskDifficulty Difficulty) : IRequest<ResponseCreateRoom>;