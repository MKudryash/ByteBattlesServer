// AuthService.Infrastructure/Data/SeedService.cs
using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Data;

public class SeedService : ISeedService
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<SeedService> _logger;

    public SeedService(IRoleRepository roleRepository, ILogger<SeedService> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task SeedRolesAsync()
    {
        try
        {
            var defaultRoles = new[]
            {
                new Role ( "admin", "Администратор"),
                new Role ( "student", "Студент"),
                new Role ( "teacher", "Преподаватель"),
            };

            var rolesAdded = 0;

            foreach (var role in defaultRoles)
            {
                if (!await _roleRepository.ExistsAsync(role.Name))
                {
                    await _roleRepository.AddAsync(role);
                    rolesAdded++;
                    _logger.LogInformation("Added Role: {RoleName}", role.Name);
                }
            }

            if (rolesAdded > 0)
            {
                await _roleRepository.SaveChangesAsync();
                _logger.LogInformation("Successfully added {Count} roles", rolesAdded);
            }
            else
            {
                _logger.LogInformation("All default roles already exist");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding roles");
            throw;
        }
    }
}