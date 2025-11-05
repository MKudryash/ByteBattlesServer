using System.Reflection;
using System.Text;
using ByteBattlesServer.Microservices.Middleware;
using ByteBattlesServer.Microservices.TaskServices.API.EndPoints;
using ByteBattlesServer.Microservices.TaskServices.API.Messaging;
using ByteBattlesServer.Microservices.TaskServices.Application;
using ByteBattlesServer.Microservices.TaskServices.Infrastructure;
using ByteBattlesServer.Microservices.TaskServices.Infrastructure.Data;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Jwt;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация RabbitMQ
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMQSettings>>().Value);
//builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();


// Конфигурация JWT
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
{
    throw new InvalidOperationException("JWT configuration is invalid.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token successfully validated");
                return Task.CompletedTask;
            }
        };
    });

// Регистрация сервисов
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();

// // Регистрация RabbitMQ с Resilient оберткой
builder.Services.AddSingleton<IMessageBus>(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<ResilientMessageBus>>();
    var logger1 = serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>();
    var settings = serviceProvider.GetRequiredService<RabbitMQSettings>();
    var messageBus = new RabbitMQMessageBus(settings,logger1);
    return new ResilientMessageBus(messageBus, logger);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Service API",
        Version = "v1",
        Description = "Task Management Service"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

// Регистрируем LanguageMessageHandler как Singleton, если он не зависит от scoped сервисов
builder.Services.AddSingleton<LanguageMessageHandler>();
builder.Services.AddSingleton<TaskMessageHandler>();

// builder.Services.AddSingleton<IMessageBus>(serviceProvider =>
// {
//     var logger = serviceProvider.GetRequiredService<ILogger<ResilientMessageBus>>();
//     var settings = serviceProvider.GetRequiredService<RabbitMQSettings>();
//     var messageBus = new RabbitMQMessageBus(settings);
//     return new ResilientMessageBus(messageBus, logger);
// });

// LanguageMessageHandler регистрируется как Hosted Service
builder.Services.AddHostedService<LanguageMessageHandler>();
builder.Services.AddHostedService<TaskMessageHandler>();

var app = builder.Build();

// Конфигурация middleware
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Service API v1");
    options.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();



app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

// Миграции базы данных
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    context.Database.Migrate();
}

app.MapTaskEndpoints();
app.MapLanguageEndpoints();

app.Run();