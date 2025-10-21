using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.API.EndPoints;

public static class LanguageEndpoints
{
    public static void MapLanguageEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/language")
            .WithTags("Language");


        group.MapGet("/{languageId:guid}", async (Guid languageId, IMediator mediator) =>
        {
            try
            {
                var query = new GetLanguageByIdQuery(languageId);
                var result = await mediator.Send(query);
                return Results.Ok(result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });

        group.MapPost("/", async (CreateLanguageDto dto, IMediator mediator, HttpContext http) =>
        {
            try
            {
                var command = new CreateLanguageCommand(dto.Title, dto.ShortTitle);
                var result = await mediator.Send(command);
                return Results.Created($"api/language/{result.Id}", result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
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