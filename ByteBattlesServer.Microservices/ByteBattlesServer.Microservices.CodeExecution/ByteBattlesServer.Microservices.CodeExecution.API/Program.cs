using ByteBattlesServer.Microservices.CodeExecution.API;
using ByteBattlesServer.Microservices.CodeExecution.Application;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using ByteBattlesServer.Microservices.CodeExecution.Infrastructure;
using ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;
using ByteBattlesServer.Microservices.Middleware;
using ByteBattlesServer.SharedContracts.Messaging;
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


// Добавление сервисов
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// // Add authentication and authorization services
// builder.Services.AddAuthentication() // Add this line
//     .AddCookie(); // Or your preferred authentication scheme

//builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Compiler Service API",
        Version = "v1",
        Description = "Task Management Service"
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ILanguageService, RabbitMQLanguageService>();
//builder.Services.AddHostedService();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Compiler Service API v1");
    options.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();


app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseRouting();
//app.UseAuthentication(); 
//app.UseAuthorization();




app.MapCompilerEndpoints();

app.Run();