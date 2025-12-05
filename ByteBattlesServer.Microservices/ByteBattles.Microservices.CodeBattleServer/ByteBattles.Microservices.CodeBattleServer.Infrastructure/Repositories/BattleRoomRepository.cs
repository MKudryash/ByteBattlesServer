using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using ByteBattles.Microservices.CodeBattleServer.Domain.Enums;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure.Repositories;


public class BattleRoomRepository : IBattleRoomRepository
{
    private readonly ApplicationDbContext _context;

    public BattleRoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BattleRoom?> GetByIdAsync(Guid id)
    {
        return await _context.BattleRooms
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<BattleRoom?> GetByIdWithParticipantsAsync(Guid id)
    {
        return await _context.BattleRooms
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<BattleRoom?> GetByIdWithSubmissionsAsync(Guid id)
    {
        return await _context.BattleRooms
            .Include(r => r.Submissions)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<BattleRoom?> GetByIdWithAllAsync(Guid id)
    {
        return await _context.BattleRooms
            .Include(r => r.Participants)
            .Include(r => r.Submissions)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<BattleRoom>> GetAllAsync()
    {
        return await _context.BattleRooms
            .ToListAsync();
    }

    public async Task<IEnumerable<BattleRoom>> GetByStatusAsync(RoomStatus status)
    {
        return await _context.BattleRooms
            .Where(r => r.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<BattleRoom>> GetByUserIdAsync(Guid userId)
    {
        return await _context.BattleRooms
            .Include(r => r.Participants)
            .Where(r => r.Participants.Any(p => p.UserId == userId))
            .ToListAsync();
    }

    public async Task AddAsync(BattleRoom room)
    {
        await _context.BattleRooms.AddAsync(room);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BattleRoom room)
    {
        _context.BattleRooms.Update(room);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var room = await GetByIdAsync(id);
        if (room != null)
        {
            _context.BattleRooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.BattleRooms
            .AnyAsync(r => r.Id == id);
    }

    public async Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId)
    {
        return await _context.BattleRooms
            .Include(r => r.Participants)
            .Where(r => r.Id == roomId)
            .SelectMany(r => r.Participants)
            .AnyAsync(p => p.UserId == userId);
    }
}