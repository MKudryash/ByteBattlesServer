
using ByteBattlesServer.Microservices.Middleware;
using ByteBattlesServer.Microservices.SolutionService.API;
using ByteBattlesServer.Microservices.SolutionService.Application;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

// ИСПРАВЛЕНИЕ: Добавьте эту строку для регистрации самого RabbitMQSettings
builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMQSettings>>().Value);


builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();
//
// var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
// var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
// if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
// {
//     throw new InvalidOperationException("JWT configuration is invalid.");
// }
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtSettings.Issuer,
//             ValidAudience = jwtSettings.Audience,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
//         };
//         options.Events = new JwtBearerEvents
//         {
//             OnAuthenticationFailed = context =>
//             {
//                 Console.WriteLine($"Authentication failed: {context.Exception.Message}");
//                 return Task.CompletedTask;
//             },
//             OnTokenValidated = context =>
//             {
//                 Console.WriteLine("Token successfully validated");
//                 return Task.CompletedTask;
//             }
//         };
//     });


// Регистрация JWT настроек
//builder.Services.AddSingleton(jwtSettings);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(
                "http://hobbit1021.ru:50306",
                "http://localhost:8080",
                "http://localhost:50306" 
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// Добавление сервисов
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Solution Service API",
        Version = "v1",
        Description = "Solution Management Service"
    });
});
builder.Services.AddAuthentication();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITestCasesServices, RabbitMQTestCasesService>();
builder.Services.AddScoped<ICompilationService, RabbitMqCompilationService>();

var services = builder.Services;

services.Configure<CompilerServiceOptions>(options =>
{
    builder.Configuration.GetSection("CompilerService").Bind(options);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Solution Service API v1");
    options.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();


app.UseCors("AllowSpecificOrigin");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SolutionDbContext>();
    context.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseRouting();
app.UseAuthentication();




 app.MapSolutionEndpoints();

app.Run();