using System.Security.Claims;
using ByteBattlesServer.Microservices.Middleware.Exceptions;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

namespace ByteBattlesServer.Microservices.TaskServices.API.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeAttribute : Attribute
{
    public string[] Roles { get; set; } = Array.Empty<string>();

    public void ValidateAuthorization(HttpContext context)
    {
        // Проверка аутентификации
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        // Проверка ролей
        if (Roles.Any() && !Roles.Any(role => context.User.IsInRole(role)))
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
                
            throw new ForbiddenAccessException(
                $"Required roles: {string.Join(", ", Roles)}. " +
                $"User roles: {string.Join(", ", userRoles)}");
        }
    }
}