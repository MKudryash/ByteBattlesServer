namespace ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;

public interface ISeedService
{
    Task SeedRolesAsync();
}