using Microsoft.Extensions.DependencyInjection;

namespace ByteBattlesServer.Microservices.UserProfile.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Регистрируем MediatR и все обработчики из текущей сборки
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}