using ByteBattlesServer.Microservices.Middleware;
using ByteBattlesServer.Microservices.UserProfile.API;
using ByteBattlesServer.Microservices.UserProfile.Application;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add authentication and authorization services
builder.Services.AddAuthentication() // Add this line
    .AddCookie(); // Or your preferred authentication scheme

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Byte API V1");
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();

// Глобальная обработка исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Add routing, authentication, and authorization middleware
app.UseRouting();
app.UseAuthentication(); // Add this line
app.UseAuthorization();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserProfileDbContext>();
    context.Database.Migrate();

    // Seed initial data if needed
    //await SeedData.InitializeAsync(context);
}

// Регистрация endpoints
app.MapUserProfileEndpoints();

app.Run();