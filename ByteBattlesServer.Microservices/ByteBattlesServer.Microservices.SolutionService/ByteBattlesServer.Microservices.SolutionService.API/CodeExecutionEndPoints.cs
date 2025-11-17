using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.Middleware.Exceptions;
using ByteBattlesServer.Microservices.SolutionService.Application.Commands;
using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using ByteBattlesServer.Microservices.SolutionService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

namespace ByteBattlesServer.Microservices.SolutionService.API;

public static class SolutionEndpoints
{
    public static void MapSolutionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/solution")
            .WithTags("Solution");
        
        // Тестирование кода с набором тестов
        group.MapPost("/test", async (SubmitSolutionDto dto, IMediator mediator,HttpContext http) =>
            {
                try
                {
                    ValidateUserOrAdminOrTeacherAccess(http);
                    var userId = GetUserIdFromClaims(http);
                    var command = new SubmitSolutionCommand(
                        dto.TaskId,
                        dto.Difficulty,
                        userId,
                        dto.LanguageId,
                        dto.Code);

                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred while testing code: {ex.Message}");
                }
            })
            .WithName("TestCode")
            .WithSummary("Тестирование кода")
            .WithDescription("Отправляет решение задачи на проверку, запускает выполнение кода с тестовыми наборами и возвращает результаты тестирования")
            .Produces<SolutionDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/statistic", async ( IMediator mediator, HttpContext http) =>
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                var userId = GetUserIdFromClaims(http);
                var command = new GetUserStatisticsQuery(userId);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("StatisticUser")
            .WithSummary("Статистика пользователя")
            .WithDescription("Возвращает общую статистику пользователя по всем решенным задачам, включая количество решенных задач, успешных решений и общую эффективность")
            .Produces<SolutionStatisticsDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);        
        
        group.MapPost("/taskSolution", async (SolutionTaskAndUserDto dto, IMediator mediator, HttpContext http) =>
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                var userId = GetUserIdFromClaims(http);
                var command = new GetTaskSolutionsQuery(dto.TaskId, userId);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("TaskUserSolution")
            .WithSummary("Статистика пользователя по задаче")
            .WithDescription("Возвращает все решения конкретного пользователя для указанной задачи, включая историю попыток, статусы и результаты выполнения")
            .Produces<SolutionDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        
        group.MapGet("/userSolution", async (IMediator mediator, HttpContext http) =>
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                var userId = GetUserIdFromClaims(http);
                var command = new GetUserSolutionsQuery(userId);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("UserSolutions")
            .WithSummary("Статистика пользователя по задачам")
            .WithDescription("Возвращает все решения пользователя по всем задачам, включая подробную информацию о каждой попытке, времени выполнения и результатах тестирования")
            .Produces<SolutionDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
       private static Guid GetUserIdFromClaims(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? context.User.FindFirst("sub")?.Value
                          ?? context.User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            var allClaims = context.User.Claims.Select(c => $"{c.Type}: {c.Value}");
            Console.WriteLine("Available claims: " + string.Join(", ", allClaims));
            throw new UnauthorizedAccessException("User ID not found in claims");
        }

        return Guid.Parse(userIdClaim);
    }
    private static string GetUserNameFromClaims(HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Email)?.Value 
               ?? context.User.FindFirst(ClaimTypes.Name)?.Value 
               ?? "Unknown";
    }
    private static void ValidateAdminAccess(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            
            throw new UnauthorizedAccessException("Authentication required to perform this action");
        }

        if (!context.User.IsInRole("admin"))
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
                    
            throw new ForbiddenAccessException(
                $"Required role: Admin. User roles: {string.Join(", ", userRoles)}");
        }
    } 
    private static void ValidateUserOrAdminOrTeacherAccess(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            
            throw new UnauthorizedAccessException("Authentication required to perform this action");
        }

        if (!context.User.IsInRole("admin")&& !context.User.IsInRole("teacher")&& !context.User.IsInRole("user"))
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
                    
            throw new ForbiddenAccessException(
                $"Required role: Admin. User roles: {string.Join(", ", userRoles)}");
        }
    }
    
    private static void ValidateAdminOrTeacherAccess(HttpContext context)
    {

        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("Authentication required to perform this action");
        }

        if (!context.User.IsInRole("admin")&& !context.User.IsInRole("teacher"))
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
                    
            throw new ForbiddenAccessException(
                $"Required role: Admin or Teacher. User roles: {string.Join(", ", userRoles)}");
        }
    }
}