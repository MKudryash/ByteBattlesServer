using System.Text;
using ByteBattlesServer.Microservices.Middleware;
using ByteBattlesServer.Microservices.SolutionService.API;
using ByteBattlesServer.Microservices.SolutionService.Application;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;
using ByteBattlesServer.SharedContracts.Jwt;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value);

builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

// JWT Authentication
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
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton(jwtSettings);

// CORS
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

// Сервисы приложения
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Solution Service API",
        Version = "v1",
        Description = "Solution Management Service"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITaskInfoServices, RabbitMQTaskInfoService>();
builder.Services.AddScoped<ICompilationService, RabbitMqCompilationService>();

var services = builder.Services;
services.Configure<CompilerServiceOptions>(options =>
{
    builder.Configuration.GetSection("CompilerService").Bind(options);
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Solution Service API V1");
        c.RoutePrefix = String.Empty;
    });
}

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
app.UseAuthorization();

app.MapSolutionEndpoints();

app.Run();