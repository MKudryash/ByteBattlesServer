using ByteBattlesServer.Microservices.UserProfile.Application.Handlers;
using ByteBattlesServer.Microservices.UserProfile.Application.IntegrationEvents;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Repositories;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Services;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<UserProfileDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();

        // Integration Events
        services.AddScoped<IIntegrationEventService, IntegrationEventService>();
        services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, UserRegisteredIntegrationEventHandler>();

        return services;
    }
}