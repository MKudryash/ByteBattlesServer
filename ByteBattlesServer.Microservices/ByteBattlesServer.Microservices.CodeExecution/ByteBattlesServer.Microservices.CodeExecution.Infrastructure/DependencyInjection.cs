using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICodeGenerator, CodeGeneratorFactory>();
        services.AddScoped<ICodeCompiler, ProcessService>();
        services.AddScoped<ITestRunner, TestRunner>();
        services.AddScoped<IFileService, FileService>();
        
        return services;
    }

}