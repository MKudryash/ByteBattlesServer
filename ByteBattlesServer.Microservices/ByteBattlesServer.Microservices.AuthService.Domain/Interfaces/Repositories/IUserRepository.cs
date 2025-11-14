using ByteBattlesServer.Microservices.AuthService.Domain.Entities;

namespace ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetByEmailWithRolesAsync(string email); // Новый метод
    Task AddAsync(User user);
    void Update(User user);
    Task SaveChangesAsync();
}