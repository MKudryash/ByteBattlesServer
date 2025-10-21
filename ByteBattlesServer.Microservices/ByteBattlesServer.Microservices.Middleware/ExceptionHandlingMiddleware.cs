using System.Text.Json;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.AuthService.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AuthException ex)
        {
            _logger.LogWarning(ex, "Auth exception occurred");
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest,
                new { message = ex.Message, code = ex.ErrorCode });
        }
        catch (UserProfileException ex)
        {
            _logger.LogWarning(ex, "User exception occurred");
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest,
                new { message = ex.Message, code = ex.ErrorCode });
        } 
        catch (TaskException ex)
        {
            _logger.LogWarning(ex, "Task exception occurred");
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest,
                new {essage = ex.Message, code = ex.ErrorCode });
        } 
        catch (LanguageException ex)
        {
            _logger.LogWarning(ex, "Language exception occurred");
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest,
                new {essage = ex.Message, code = ex.ErrorCode });
        }
        catch (ErrorRequest ex)
        {
            _logger.LogWarning(ex, "Error exception occurred");
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest,
                new { message = ex.Message });
        }

        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation exception occurred");
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest,
                new { message = "Validation failed", errors = ex.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError,
                new { message = "Internal server error", code = "INTERNAL_ERROR" });
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, object response)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
        
        await context.Response.WriteAsync(json);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}