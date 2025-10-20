// UserProfile.API/Endpoints/UserProfileEndpoints.cs
using System.Security.Claims;
using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using MediatR;
using Microsoft.IdentityModel.JsonWebTokens;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace ByteBattlesServer.Microservices.UserProfile.API;

public static class UserProfileEndpoints
{
    public static void MapUserProfileEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/user-profiles")
            .WithTags("User Profiles");
        
        
        // Get current user's profile
        group.MapGet("/me", async (IMediator mediator, HttpContext httpContext) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                var query = new GetUserProfileQuery(userId);
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

        // Update user profile
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

        // Update user settings
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

        // Get user profile by ID
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

        // Create user profile (для тестирования или админки)
        group.MapPost("/", async (CreateUserProfileCommandDto dto, IMediator mediator) =>
        {
            try
            {
                var command = new CreateUserProfileCommand(dto.UserId, dto.UserName);
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

// DTO для параметров запроса
public record LeaderboardQueryParams(int Page = 1, int PageSize = 20);

public record SearchQueryParams(string SearchTerm, int Page = 1, int PageSize = 10);

// DTO для создания профиля
public record CreateUserProfileCommandDto(Guid UserId, string UserName);

// Response DTO для ошибок
public record ErrorResponse(string Message, string Code);