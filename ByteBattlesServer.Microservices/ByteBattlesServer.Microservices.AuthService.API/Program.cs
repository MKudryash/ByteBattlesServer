// Program.cs

using System.Text;
using ByteBattlesServer.Microservices.AuthService.API;
using ByteBattlesServer.Microservices.AuthService.Infrastructure;
using ByteBattlesServer.Microservices.AuthService.Infrastructure.Data;
using ByteBattlesServer.Microservices.AuthServices.Application;
using ByteBattlesServer.Microservices.Middleware;
using ByteBattlesServer.SharedContracts.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ByteBattlesServer.SharedContracts.Messaging;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

// ИСПРАВЛЕНИЕ: Добавьте эту строку для регистрации самого RabbitMQSettings
builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMQSettings>>().Value);


builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

// Добавление сервисов
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(
                "http://hobbit1021.ru",
                "http://localhost:8080",
                "http://localhost:50305"// Ваш production домен
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Если используете cookies/авторизацию
    });

    // Альтернативно, для разработки можно разрешить все origins
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Регистрация JwtSettings как singleton
builder.Services.AddSingleton(jwtSettings);
// Add JWT Authentication
builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
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
    });

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Byte API V1");
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

// Глобальная обработка исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

// ДОБАВЛЕНО: Аутентификация и авторизация
app.UseAuthentication();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    // Seed initial data if needed
    //await SeedData.InitializeAsync(context);
}

// Регистрация endpoints
app.MapAuthEndpoints();

app.Run();