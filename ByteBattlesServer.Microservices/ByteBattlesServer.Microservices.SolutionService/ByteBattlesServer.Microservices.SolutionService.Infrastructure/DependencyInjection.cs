using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddScoped<ICompilationService, CompilationService>();
        services.AddScoped<ISolutionRepository, ISolutionRepository>();
       
        return services;
    }
}

// public static class ServiceCollectionExtensions
// {
//     public static IServiceCollection AddCompilerClient(this IServiceCollection services, IConfiguration configuration)
//     {
//         services.Configure<CompilerServiceOptions>(
//             configuration.GetSection("CompilerService"));
//         
//         services.AddHttpClient<ICompilerClient, CompilerClient>((serviceProvider, client) =>
//             {
//                 var options = serviceProvider.GetRequiredService<IOptions<CompilerServiceOptions>>().Value;
//                 client.BaseAddress = new Uri(options.BaseUrl);
//                 client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
//             })
//             .AddPolicyHandler(GetRetryPolicy())
//             .AddPolicyHandler(GetCircuitBreakerPolicy());
//
//         return services;
//     }
//
//     private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//     {
//         return HttpPolicyExtensions
//             .HandleTransientHttpError()
//             .OrResult(msg => !msg.IsSuccessStatusCode)
//             .WaitAndRetryAsync(
//                 retryCount: 3,
//                 sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
//                 onRetry: (outcome, timespan, retryCount, context) =>
//                 {
//                     var logger = context.GetLogger<CompilerClient>();
//                     logger.LogWarning("Retry {RetryCount} for compiler service after {Delay}ms", retryCount, timespan.TotalMilliseconds);
//                 });
//     }
//
//     private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
//     {
//         return HttpPolicyExtensions
//             .HandleTransientHttpError()
//             .CircuitBreakerAsync(
//                 handledEventsAllowedBeforeBreaking: 3,
//                 durationOfBreak: TimeSpan.FromSeconds(30));
//     }
// }