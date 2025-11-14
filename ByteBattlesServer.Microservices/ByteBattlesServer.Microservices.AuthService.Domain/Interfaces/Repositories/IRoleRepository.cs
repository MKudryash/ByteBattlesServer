using ByteBattlesServer.Microservices.AuthService.Domain.Entities;

namespace ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;

public interface IRoleRepository
{
    
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames);
    Task AddAsync(Role role);
    void Update(Role role);
    void Delete(Role role);
    Task<bool> ExistsAsync(string name);
    Task<bool> ExistsAsync(Guid id);
    Task<int> SaveChangesAsync();
}