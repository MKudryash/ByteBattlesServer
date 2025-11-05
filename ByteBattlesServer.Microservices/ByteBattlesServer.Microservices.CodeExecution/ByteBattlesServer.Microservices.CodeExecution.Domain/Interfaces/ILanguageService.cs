using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

public interface ILanguageService
{
    Task<LanguageInfo> GetLanguageInfoAsync(Guid languageId);
}