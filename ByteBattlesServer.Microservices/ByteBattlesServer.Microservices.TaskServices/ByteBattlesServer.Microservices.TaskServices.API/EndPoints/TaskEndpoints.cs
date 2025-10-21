using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.API.EndPoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/task")
            .WithTags("Task");


        group.MapGet("/{taskId:guid}", async (Guid taskId, IMediator mediator) =>
        {
            try
            {
               var query = new GetTaskByIdQuery(taskId);
               var result = await mediator.Send(query);
               return Results.Ok(result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });

        group.MapPost("/", async (CreateTaskDto dto, IMediator mediator, HttpContext http) =>
        {
            try
            {
                var command = new CreateTaskCommand(dto.Title, dto.Description,
                    dto.Difficulty, dto.Author,
                    dto.FunctionName, dto.InputParameters,
                    dto.OutputParameters, DateTime.Now, dto.LanguageId);
                var result = await mediator.Send(command);
                return Results.Created($"api/task/{result.Id}", result);
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