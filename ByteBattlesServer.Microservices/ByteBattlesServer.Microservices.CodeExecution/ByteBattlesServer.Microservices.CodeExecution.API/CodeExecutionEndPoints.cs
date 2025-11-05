using System.ComponentModel.DataAnnotations;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.CodeExecution.Application.Commands;
using ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.CodeExecution.API;


public static class CompilerEndpoints
{
    public static void MapCompilerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/compiler")
            .WithTags("Compiler");
        
        // Тестирование кода с набором тестов
        group.MapPost("/test", async (CodeSubmissionRequest dto, IMediator mediator) =>
            {
                try
                {
                    var command = new TestCodeCommand(
                        dto.Code,
                        dto.Language,
                        dto.TestCases);

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
            .Produces<CodeTestResultResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

    }
}