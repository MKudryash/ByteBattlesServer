using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using ByteBattles.Microservices.CodeBattleServer.Domain.Enums;

namespace ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;

public interface IBattleRoomRepository
{
    Task<BattleRoom?> GetByIdAsync(Guid id);
    Task<BattleRoom?> GetByIdWithParticipantsAsync(Guid id);
    Task<BattleRoom?> GetByIdWithSubmissionsAsync(Guid id);
    Task<BattleRoom?> GetByIdWithAllAsync(Guid id);
    Task<IEnumerable<BattleRoom>> GetAllAsync();
    Task<IEnumerable<BattleRoom>> GetByStatusAsync(RoomStatus status);
    Task<IEnumerable<BattleRoom>> GetByUserIdAsync(Guid userId);
    Task AddAsync(BattleRoom room);
    Task UpdateAsync(BattleRoom room);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId);
}