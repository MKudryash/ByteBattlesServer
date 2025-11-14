using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames)
    {
        return await _context.Roles
            .Where(r => roleNames.Contains(r.Name))
            .ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public void Update(Role role)
    {
        _context.Roles.Update(role);
    }

    public void Delete(Role role)
    {
        _context.Roles.Remove(role);
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _context.Roles
            .AnyAsync(r => r.Name == name);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Roles
            .AnyAsync(r => r.Id == id);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}