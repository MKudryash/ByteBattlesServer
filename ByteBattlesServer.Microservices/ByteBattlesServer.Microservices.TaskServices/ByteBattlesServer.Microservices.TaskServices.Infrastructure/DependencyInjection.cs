using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using ByteBattlesServer.Microservices.TaskServices.Infrastructure.Data;
using ByteBattlesServer.Microservices.TaskServices.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<TaskDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        
        return services;
    }

}