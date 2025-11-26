using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure.Repositories;

public class CodeSubmissionRepository : ICodeSubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public CodeSubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CodeSubmission?> GetByIdAsync(Guid id)
    {
        return await _context.CodeSubmissions
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<CodeSubmission>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.CodeSubmissions
            .Where(s => s.RoomId == roomId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CodeSubmission>> GetByUserIdAsync(Guid userId)
    {
        return await _context.CodeSubmissions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CodeSubmission>> GetByRoomAndUserAsync(Guid roomId, Guid userId)
    {
        return await _context.CodeSubmissions
            .Where(s => s.RoomId == roomId && s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CodeSubmission>> GetByProblemIdAsync(string problemId)
    {
        return await _context.CodeSubmissions
            .Where(s => s.ProblemId == problemId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<CodeSubmission?> GetLatestByRoomAndUserAsync(Guid roomId, Guid userId)
    {
        return await _context.CodeSubmissions
            .Where(s => s.RoomId == roomId && s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CodeSubmission submission)
    {
        await _context.CodeSubmissions.AddAsync(submission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CodeSubmission submission)
    {
        _context.CodeSubmissions.Update(submission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var submission = await GetByIdAsync(id);
        if (submission != null)
        {
            _context.CodeSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.CodeSubmissions
            .AnyAsync(s => s.Id == id);
    }
}