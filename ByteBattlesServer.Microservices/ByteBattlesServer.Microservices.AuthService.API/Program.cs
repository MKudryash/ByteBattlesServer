// Program.cs

using System.Text;
using ByteBattlesServer.Microservices.AuthService.API;
using ByteBattlesServer.Microservices.AuthService.Domain.Configuration;
using ByteBattlesServer.Microservices.AuthService.Infrastructure;
using ByteBattlesServer.Microservices.AuthService.Infrastructure.Data;
using ByteBattlesServer.Microservices.AuthServices.Application;
using ByteBattlesServer.Microservices.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

// Добавление сервисов
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Byte API V1");
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();

// Глобальная обработка исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();


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