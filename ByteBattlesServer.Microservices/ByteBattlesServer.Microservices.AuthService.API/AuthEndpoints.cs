// AuthService.API/Endpoints/AuthEndpoints.cs

using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.AuthService.Domain.Exceptions;
using ByteBattlesServer.Microservices.AuthServices.Application.Commands;
using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace ByteBattlesServer.Microservices.AuthService.API;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth")
            .WithTags("Authentication");

        // Регистрация
        group.MapPost("/register", async (
            RegisterDto registerDto, IMediator mediator) =>
        {
            try
            {
                var command = new RegisterCommand(
                    registerDto.Email,
                    registerDto.Password,
                    registerDto.FirstName,
                    registerDto.LastName);

                var result = await mediator.Send(command);
                Console.WriteLine(result);
                return Results.Ok(result);
            }
            catch (AuthException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ex.ValidationResult.ErrorMessage);
            }
        })
        .WithName("Register")
        .WithSummary("Регистрация нового пользователя")
        .WithDescription("Создает нового пользователя и возвращает токены доступа");

        // Логин
        group.MapPost("/login", async (
            LoginDto loginDto, IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var ipAddress = GetIpAddress(httpContext);
                var command = new LoginCommand(
                    loginDto.Email,
                    loginDto.Password,
                    ipAddress);

                var result = await mediator.Send(command);
                Console.WriteLine(result);
                return Results.Ok(result);
            }
            catch (AuthException ex)
            {
                return Results.Unauthorized();
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ex.ValidationResult.ErrorMessage);
            }
        })
        .WithName("Login")
        .WithSummary("Аутентификация пользователя")
        .WithDescription("Проверяет учетные данные и возвращает токены доступа");

        // Refresh token
        group.MapPost("/refresh-token", async (
            RefreshTokenRequest request, IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var ipAddress = GetIpAddress(httpContext);
                var command = new RefreshTokenCommand(
                    request.AccessToken,
                    request.RefreshToken,
                    ipAddress);

                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (AuthException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (SecurityTokenException ex)
            {
                return Results.BadRequest(new ErrorResponse("Invalid token", "INVALID_TOKEN"));
            }
        })
        .WithName("RefreshToken")
        .WithSummary("Обновление токена доступа")
        .WithDescription("Обновляет access token используя refresh token");

        // Revoke token
        group.MapPost("/revoke-token", async (
            RevokeTokenRequest request, IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var ipAddress = GetIpAddress(httpContext);
                var command = new RevokeTokenCommand(
                    request.RefreshToken,
                    ipAddress);

                await mediator.Send(command);
                return Results.Ok();
            }
             catch (AuthException ex)
             {
                 return Results.BadRequest(new ErrorResponse(ex.Message, ex.ErrorCode));
             }
        })
        .WithName("RevokeToken")
        .WithSummary("Отзыв refresh token")
        .WithDescription("Отзывает refresh token, делая его недействительным");
    }

    private static string GetIpAddress(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            return httpContext.Request.Headers["X-Forwarded-For"]!;
        
        return httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";
    }
}

public record RefreshTokenRequest(string AccessToken, string RefreshToken);
public record RevokeTokenRequest(string RefreshToken);