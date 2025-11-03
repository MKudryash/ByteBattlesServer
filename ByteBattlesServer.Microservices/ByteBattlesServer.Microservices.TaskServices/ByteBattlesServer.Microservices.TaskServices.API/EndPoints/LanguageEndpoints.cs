using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.API.EndPoints;

public static class LanguageEndpoints
{
    public static void MapLanguageEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/language")
            .WithTags("Language");

        // Получение языка по ID
        group.MapGet("/{languageId:guid}", async (Guid languageId, IMediator mediator) =>
            {
                try
                {
                    var query = new GetLanguageByIdQuery(languageId);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
                catch (LanguageNotFoundException ex)
                {
                    return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred while retrieving language: {ex.Message}");
                }
            })
            .WithName("GetLanguageById")
            .WithSummary("Получить язык по идентификатору")
            .WithDescription("Извлекает определенный язык программирования по его уникальному идентификатору")
            .Produces<LanguageDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // Создание нового языка
        group.MapPost("/", async (CreateLanguageDto dto, IMediator mediator, HttpContext http) =>
            {
                try
                {
                    var command = new CreateLanguageCommand(dto.Title, dto.ShortTitle, dto.FileExtension,
                        dto.CompilerCommand, dto.ExecutionCommand,dto.SupportsCompilation);
                    var result = await mediator.Send(command);
                    return Results.Created($"/api/language/{result.Id}", result);
                }
                catch (LanguageAlreadyExistsException ex)
                {
                    return Results.Conflict(new ErrorResponse(ex.Message, ex.ErrorCode));
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred while creating language: {ex.Message}");
                }
            })
            .WithName("CreateLanguage")
            .WithSummary("Создание нового языка программирования")
            .WithDescription("Создание новый язык программирования с названием и кратким заголовком")
            .Produces<LanguageDto>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // Обновление языка
        group.MapPut("/", async (UpdateLanguageDto dto, IMediator mediator, HttpContext http) =>
            {
                try
                {
                    var command = new UpdateLanguageCommand(dto.Id, dto.Title, dto.ShortTitle, dto.FileExtension,
                        dto.CompilerCommand, dto.ExecutionCommand,dto.SupportsCompilation);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
                catch (LanguageNotFoundException ex)
                {
                    return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred while updating language: {ex.Message}");
                }
            })
            .WithName("UpdateLanguage")
            .WithSummary("Обновление языка программирования")
            .WithDescription("Обновляет название существующего языка программирования и краткое название")
            .Produces<LanguageDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);


        group.MapGet("/search-paged",
                async (IMediator mediator, [AsParameters] TaskLanguageFilterPagedDto taskTaskFilter) =>
                {
                    try
                    {
                        var query = new SearchLanguagesPagedQuery(taskTaskFilter.SearchTerm,
                            taskTaskFilter.Page, taskTaskFilter.PageSize);
                        var result = await mediator.Send(query);
                        return Results.Ok(result);
                    }
                    catch (LanguageNotFoundException ex)
                    {
                        return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
                    }
                    catch (ValidationException ex)
                    {
                        return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
                    }
                })
            .WithName("GetAllLanguagesPaged")
            .WithSummary("Получение списка языков программирования с пагинацией")
            .WithDescription(
                "Извлекает все языки программирования с дополнительной фильтрацией и разбивкой по страницам")
            .Produces<List<LanguageDto>>(StatusCodes.Status200OK);

        group.MapGet("/search", async (IMediator mediator, [AsParameters] LanguageFilerDto taskFilter) =>
            {
                try
                {
                    var query = new SearchLanguagesQuery(taskFilter.SearchTerm);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
                catch (LanguageNotFoundException ex)
                {
                    return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
                }
                catch (Exception ex)
                {
                    return Results.Problem($"An error occurred while retrieving tasks: {ex.Message}");
                }
            })
            .WithName("GetAllLanguages")
            .WithSummary("Получение списка языков программирования")
            .WithDescription("Извлекает все языки программирования с дополнительной фильтрацией")
            .Produces<List<LanguageDto>>(StatusCodes.Status200OK);


        group.MapDelete("/{languageId:guid}", async (Guid languageId, IMediator mediator) =>
        {
            try
            {
                var command = new RemoveLanguageCommand(languageId);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (LanguageInUseException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (LanguageNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving language: {ex.Message}");
            }
        })
        .WithName("DeleteLanguages")
        .WithSummary("Удаления языка программирования")
        .WithDescription("Удаляет язык программирования по его уникальному идентификатору")
        .Produces<List<LanguageDto>>(StatusCodes.Status200OK);;
    
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