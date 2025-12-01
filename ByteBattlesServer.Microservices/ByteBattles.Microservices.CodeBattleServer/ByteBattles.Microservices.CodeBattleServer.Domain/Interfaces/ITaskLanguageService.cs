using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;

public interface ITaskLanguageService
{
    Task<TaskInfo> GetTaskInfoAsync(Guid languageId, TaskDifficulty difficulty);
}