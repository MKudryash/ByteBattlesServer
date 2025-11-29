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

builder.Services.AddSingleton<IMessageBus, ResilientMessageBus>();


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

builder.Services.AddAuthorization();

// Регистрация сервисов
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

// Регистрируем LanguageMessageHandler как Singleton, если он не зависит от scoped сервисов
builder.Services.AddSingleton<LanguageMessageHandler>();
builder.Services.AddSingleton<TaskInfoTRandomTaskMessageHandler>();
builder.Services.AddSingleton<TaskInfoTaskMessageHandler>();
builder.Services.AddSingleton<TaskMessageHandler>();

// LanguageMessageHandler регистрируется как Hosted Service
builder.Services.AddHostedService<LanguageMessageHandler>();
builder.Services.AddHostedService<TaskMessageHandler>();
builder.Services.AddHostedService<TaskInfoTaskMessageHandler>();

builder.Services.AddHostedService<TaskInfoTRandomTaskMessageHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(
                "http://hobbit1021.ru:50304",
                "http://localhost:8080",
                "http://localhost:50304"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Конфигурация middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Service API V1");
    c.RoutePrefix = "swagger"; // Это позволит API Gateway получать спецификацию
});
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowSpecificOrigin");

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