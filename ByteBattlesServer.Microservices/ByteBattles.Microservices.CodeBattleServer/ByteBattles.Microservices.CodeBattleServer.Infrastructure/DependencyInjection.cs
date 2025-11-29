using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattles.Microservices.CodeBattleServer.Domain.Services;
using ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data;
using ByteBattles.Microservices.CodeBattleServer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IBattleRoomRepository, BattleRoomRepository>();
        services.AddScoped<ICodeSubmissionRepository, CodeSubmissionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITaskLanguageService, RabbitMQTaskLanguageService>();
        services.AddScoped<ICompilationService, RabbitMqCompilationService>();
        services.AddMemoryCache();

        return services;
    }
}