using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Repositories;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<SolutionDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<ISolutionRepository, SolutionRepository>();

        // Services
        services.AddScoped<ICompilationService, RabbitMqCompilationService>();
        // services.AddScoped<ITaskServiceClient, TaskServiceClient>();
        
        // HTTP Clients Configuration
        //ConfigureHttpClients(services, configuration);

        return services;
    }
    
}