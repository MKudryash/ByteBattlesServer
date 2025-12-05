using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;

namespace ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;


public interface ICodeSubmissionRepository
{
    Task<CodeSubmission?> GetByIdAsync(Guid id);
    Task<IEnumerable<CodeSubmission>> GetByRoomIdAsync(Guid roomId);
    Task<IEnumerable<CodeSubmission>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<CodeSubmission>> GetByRoomAndUserAsync(Guid roomId, Guid userId);
    Task<IEnumerable<CodeSubmission>> GetByProblemIdAsync(Guid problemId);
    Task<CodeSubmission?> GetLatestByRoomAndUserAsync(Guid roomId, Guid userId);
    Task AddAsync(CodeSubmission submission);
    Task UpdateAsync(CodeSubmission submission);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}