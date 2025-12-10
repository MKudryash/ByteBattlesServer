// UserProfile.API/Endpoints/UserProfileEndpoints.cs
using System.Security.Claims;
using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace ByteBattlesServer.Microservices.UserProfile.API;

public static class UserProfileEndpoints
{
    public static void MapUserProfileEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/user-profiles")
            .WithTags("User Profiles");
        
        
        
        group.MapGet("/me", async (IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var roleUser = GetUserRoleFromClaims(httpContext);
                var query = new GetUserProfileQuery(userId,roleUser);
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving user profile: {ex.Message}");
            }
        })
        .RequireAuthorization()
        .WithName("GetMyProfile")
        .WithSummary("Get current user's profile")
        .Produces<UserProfileDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

       
        group.MapPut("/me", async (UpdateProfileCommandDto dto, IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var command = new UpdateUserProfileCommand(
                    userId, dto.UserName, dto.Bio, dto.Country, 
                    dto.GitHubUrl, dto.LinkedInUrl, dto.IsPublic);
                    
                await mediator.Send(command);
                return Results.Ok(new { message = "Profile updated successfully" });
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while updating profile: {ex.Message}");
            }
        })
        .RequireAuthorization()
        .WithName("UpdateMyProfile")
        .WithSummary("Update current user's profile")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        
        group.MapPut("/me/settings", async (UpdateSettingsCommandDto dto, IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var command = new UpdateUserSettingsCommand(
                    userId, dto.EmailNotifications, dto.BattleInvitations,
                    dto.AchievementNotifications, dto.Theme, dto.CodeEditorTheme, dto.PreferredLanguage);
                    
                await mediator.Send(command);
                return Results.Ok(new { message = "Settings updated successfully" });
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while updating settings: {ex.Message}");
            }
        })
        .RequireAuthorization()
        .WithName("UpdateMySettings")
        .WithSummary("Update current user's settings")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        
        group.MapPut("/me/stats", async (UserStatsCommandDto dto, IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var command = new UpdateUserStatsCommand(
                    userId, dto.isSuccessful, dto.difficulty, dto.executionTime, dto.taskId, dto.problemTitle, dto.language,dto.activityType
                    ,dto.BattleId,dto.NameOpponent,1);
                    
                await mediator.Send(command);
                return Results.Ok(new { message = "Settings updated successfully" });
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while updating settings: {ex.Message}");
            }
        })
        .RequireAuthorization()
        .WithName("UpdateMyStats")
        .WithSummary("Update current user's stats")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        
        group.MapGet("/{profileId:guid}", async (Guid profileId, IMediator mediator) =>
        {
            try
            {
                var query = new GetUserProfileByIdQuery(profileId);
                var result = await mediator.Send(query);
                
                if (!result.IsPublic)
                    throw new ProfileAccessDeniedException(profileId);
                    
                return Results.Ok(result);
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ProfileAccessDeniedException ex)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving user profile: {ex.Message}");
            }
        })
        .WithName("GetUserProfileById")
        .WithSummary("Get user profile by ID")
        .AllowAnonymous()
        .Produces<UserProfileDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden);

        
        group.MapPost("/", async (CreateUserProfileCommandDto dto, IMediator mediator) =>
        {
            try
            {
                var command = new CreateUserProfileCommand(dto.UserId, dto.UserName, dto.Email,dto.IsPublic,dto.Role);
                var result = await mediator.Send(command);
                return Results.Created($"/api/user-profiles/{result.Id}", result);
            }
            catch (UserProfileAlreadyExistsException ex)
            {
                return Results.Conflict(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while creating user profile: {ex.Message}");
            }
        })
        .AllowAnonymous()
        .WithName("CreateUserProfile")
        .WithSummary("Create new user profile")
        .Produces<UserProfileDto>(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict);
        
        
        
        group.MapGet("/leaderboard", async (IMediator mediator, [AsParameters] LeaderboardQueryParams queryParams) =>
            {
                try
                {
                    var query = new GetLeaderboardQuery(queryParams.countTop);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.Problem("An error occurred while retrieving leaderboard");
                }
            })
            .WithName("GetLeaderboard")
            .WithSummary("Get users leaderboard")
            .AllowAnonymous()
            .Produces<List<LeaderboardUserDto>>(StatusCodes.Status200OK);
        group.MapGet("/search", async (IMediator mediator, [AsParameters] SearchQueryParams queryParams) =>
        {
            try
            {
                var query = new SearchQueryParams(queryParams.SearchTerm, queryParams.Page, queryParams.PageSize);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while searching users");
            }
        })
        .WithName("SearchUsers")
        .WithSummary("Search users by name or bio")
        .AllowAnonymous()
        .Produces<List<UserProfileDto>>(StatusCodes.Status200OK);
        
        
        // Новые методы для активности и решенных задач
        group.MapGet("/me/activity", async (IMediator mediator, HttpContext httpContext, 
            [FromQuery] int? activitiesLimit, [FromQuery] int? problemsLimit) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var query = new GetUserActivityQuery(userId, activitiesLimit, problemsLimit);
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving user activity: {ex.Message}");
            }
        })
        .RequireAuthorization()
        .WithName("GetMyActivity")
        .WithSummary("Get current user's recent activity and problems")
        .Produces<UserActivityResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/me/recent-activities", async (IMediator mediator, HttpContext httpContext, 
            [FromQuery] int limit = 50) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var query = new GetRecentActivitiesQuery(userId, limit);
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving recent activities: {ex.Message}");
            }
        })
        .RequireAuthorization()
        .WithName("GetMyRecentActivities")
        .WithSummary("Get current user's recent activities")
        .Produces<List<RecentActivityDto>>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/me/recent-problems", async (IMediator mediator, HttpContext httpContext, 
            [FromQuery] int limit = 20) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var query = new GetRecentProblemsQuery(userId, limit);
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving recent problems: {ex.Message}");
            }
        })
        .RequireAuthorization()
        .WithName("GetMyRecentProblems")
        .WithSummary("Get current user's recently solved problems")
        .Produces<List<RecentProblemDto>>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // Public endpoint для получения активности других пользователей
        group.MapGet("/{profileId:guid}/activity", async (Guid profileId, IMediator mediator, 
            [FromQuery] int? activitiesLimit, [FromQuery] int? problemsLimit) =>
        {
            try
            {
                var query = new GetUserActivityQuery(profileId, activitiesLimit, problemsLimit);
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ProfileAccessDeniedException ex)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving user activity: {ex.Message}");
            }
        })
        .WithName("GetUserActivity")
        .WithSummary("Get user's recent activity and problems")
        .AllowAnonymous()
        .Produces<UserActivityResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden);
        
        
        // Public endpoint для получения наград
        group.MapGet("/achievements", async (IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var query = new GetAchievementsQuery(userId);
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (UserProfileNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ProfileAccessDeniedException ex)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving user activity: {ex.Message}");
            }
        })
        .WithName("GetUserAchievements")
        .WithSummary("Get user's achievements ")
        .AllowAnonymous()
        .Produces<AchievementDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden);
        
    }

    private static UserRole GetUserRoleFromClaims(HttpContext context)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value
                        ?? context.User.FindFirst("Role")?.Value
                        ?? context.User.FindFirst("roles")?.Value
                        ?? context.User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/Role")?.Value;

        if (string.IsNullOrEmpty(roleClaim))
        {
            var allClaims = context.User.Claims.Select(c => $"{c.Type}: {c.Value}");
            Console.WriteLine("Available claims for Role: " + string.Join(", ", allClaims));
            
            throw new UnauthorizedAccessException("Role not found in claims");
        }

        // Парсим роль, учитывая возможные варианты написания
        return roleClaim.ToLower() switch
        {
            "student" or "1" => UserRole.student,
            "teacher" or "2" => UserRole.teacher,
            "admin" or "administrator" or "3" => UserRole.admin,
            _ => UserRole.student // значение по умолчанию
        };
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
}



