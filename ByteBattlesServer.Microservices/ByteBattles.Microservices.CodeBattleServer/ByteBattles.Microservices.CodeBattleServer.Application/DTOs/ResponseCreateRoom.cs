using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattles.Microservices.CodeBattleServer.Application.DTOs;

public class ResponseCreateRoom
{
    public Guid Id { get; set; }
    public TaskInfo  TaskInfo { get; set; }
}