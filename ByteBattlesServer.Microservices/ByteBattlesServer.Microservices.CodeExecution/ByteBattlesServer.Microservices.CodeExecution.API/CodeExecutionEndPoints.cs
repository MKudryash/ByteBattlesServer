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

        // // Компиляция и выполнение кода
        // group.MapPost("/execute", async (CodeSubmissionRequest dto, IMediator mediator) =>
        //     {
        //         try
        //         {
        //             var command = new CodeSubmissionRequest(
        //                 dto.Code,
        //                 dto.Language,
        //                 dto.TestCases);
        //
        //             var result = await mediator.Send(command);
        //             return Results.Ok(result);
        //         }
        //         // catch (CompilationException ex)
        //         // {
        //         //     return Results.BadRequest(new ErrorResponse(ex.Message, ex.ErrorCode));
        //         // }
        //         catch (ValidationException ex)
        //         {
        //             return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
        //         }
        //         catch (Exception ex)
        //         {
        //             return Results.Problem($"An error occurred while executing code: {ex.Message}");
        //         }
        //     })
        //     .WithName("ExecuteCode")
        //     .WithSummary("Выполнение кода")
        //     .WithDescription("Компилирует и выполняет код на указанном языке программирования")
        //     //.Produces<CodeExecutionResultDto>(StatusCodes.Status200OK)
        //     .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

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
                // catch (CompilationException ex)
                // {
                //     return Results.BadRequest(new ErrorResponse(ex.Message, ex.ErrorCode));
                // }
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
            //.Produces<CodeTestResultDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

    }
}