using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Repositories;


public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email.Value == email);
    }
    
    public async Task<User?> GetByEmailWithRolesAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role) // Включаем роли
            .FirstOrDefaultAsync(u => u.Email.Value == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.Value == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}