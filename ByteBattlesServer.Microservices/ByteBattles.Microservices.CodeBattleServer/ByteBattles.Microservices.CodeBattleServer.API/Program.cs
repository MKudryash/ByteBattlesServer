// Program.cs
using System.Text;
using ByteBattles.Microservices.CodeBattleServer.API;
using ByteBattles.Microservices.CodeBattleServer.Application;
using ByteBattles.Microservices.CodeBattleServer.Infrastructure;
using ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data;
using ByteBattlesServer.Microservices.Middleware;
using ByteBattlesServer.SharedContracts.Jwt;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация JWT
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
if (!jwtSettingsSection.Exists())
{
    throw new InvalidOperationException("JwtSettings section is missing from configuration.");
}

var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
if (jwtSettings == null)
{
    throw new InvalidOperationException("Failed to bind JwtSettings from configuration.");
}

if (string.IsNullOrEmpty(jwtSettings.Secret))
{
    throw new InvalidOperationException("JWT Secret is not configured.");
}

if (jwtSettings.Secret.Length < 32)
{
    throw new InvalidOperationException("JWT Secret must be at least 32 characters long.");
}

Console.WriteLine($"JWT Issuer: {jwtSettings.Issuer}");
Console.WriteLine($"JWT Audience: {jwtSettings.Audience}");
Console.WriteLine($"JWT Secret length: {jwtSettings.Secret.Length}");

// Регистрация JwtSettings как singleton
builder.Services.AddSingleton(jwtSettings);

// RabbitMQ конфигурация
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value);

builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

// Добавление сервисов
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Code Battle Server API", 
        Version = "v1",
        Description = "API для программистских битв"
    });

    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter your token in the text input below.
                      Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(
                "http://hobbit1021.ru:50312",
                "http://localhost:8080",
                "http://localhost:50312",
                "http://localhost:5035"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// JWT Authentication с поддержкой WebSocket
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
        
        // Важная часть для WebSocket: обработка токена из query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Для WebSocket соединений токен может передаваться в query string
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                // Если это WebSocket endpoint и есть токен в query string
                if (!string.IsNullOrEmpty(accessToken) && 
                    path.StartsWithSegments("/api/battle"))
                {
                    context.Token = accessToken;
                }
                
                // Также проверяем Authorization header для HTTP запросов
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(context.Token) && 
                    !string.IsNullOrEmpty(authHeader) && 
                    authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

// Добавляем авторизацию
builder.Services.AddAuthorization();

// SignalR и WebSocket сервисы
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Battle Server API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");

// Глобальная обработка исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

// Включаем WebSocket поддержку
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
    AllowedOrigins = { "http://localhost:5035", "http://hobbit1021.ru:50312" }
});

// Регистрация endpoints
app.MapBattleEndpoints();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();