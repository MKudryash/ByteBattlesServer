using System.ComponentModel.DataAnnotations;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.SolutionService.Application.Commands;
using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.API;


public static class SolutionEndpoints
{
    public static void MapSolutionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/solution")
            .WithTags("Solution");
        
        // Тестирование кода с набором тестов
        group.MapPost("/test", async (SubmitSolutionDto dto, IMediator mediator) =>
            {
                try
                {
                    var command = new SubmitSolutionCommand(
                        dto.TaskId,
                        dto.UserId,
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
            .WithDescription("Запускает код на выполнение с набором тестов и возвращает результаты")
            .Produces<SolutionDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

    }
}