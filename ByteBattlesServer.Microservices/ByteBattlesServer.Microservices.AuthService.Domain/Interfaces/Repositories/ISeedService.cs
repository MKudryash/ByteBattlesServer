namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Data;

public interface ISeedService
{
    Task SeedRolesAsync();
}