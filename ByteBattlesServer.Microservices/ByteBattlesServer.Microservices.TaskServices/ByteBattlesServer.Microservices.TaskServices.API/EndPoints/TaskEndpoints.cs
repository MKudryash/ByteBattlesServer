using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.Middleware.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using MediatR;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

namespace ByteBattlesServer.Microservices.TaskServices.API.EndPoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/task")
            .WithTags("Task");

   
        
        // Получение задачи по ID
        group.MapGet("/{taskId:guid}", async (Guid taskId, IMediator mediator, HttpContext http) =>
        {
            try
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                var query = new GetTaskByIdQuery(taskId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (TaskNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving task: {ex.Message}");
            }
        })
        .WithName("GetTaskById")
        .WithSummary("Получение задачи по идентификатору")
        .WithDescription("Определяет конкретную задачу по ее уникальному идентификатору")
        .Produces<TaskDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);    
        
        group.MapGet("/count", async (IMediator mediator, HttpContext http) =>
        {
            try
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                var query = new GetTaskCountDiffictly();
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (TaskNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving task: {ex.Message}");
            }
        })
        .WithName("GetTaskCount")
        .WithSummary("Получение задачи по идентификатору")
        .WithDescription("Определяет конкретную задачу по ее уникальному идентификатору")
        .Produces<TaskCountDifficatly>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // Создание новой задачи
        group.MapPost("/", async (CreateTaskDto dto, IMediator mediator, HttpContext http) =>
        {
            try
            {
                ValidateAdminOrTeacherAccess(http);
                
                var command = new CreateTaskCommand(
                    dto.Title, 
                    dto.Description,
                    dto.Difficulty, 
                    dto.Author,
                    dto.FunctionName, 
                    dto.PatternMain,
                    dto.PatternFunction, 
                    dto.Parameters,
                    dto.ReturnType,
                    dto.LanguageId,
                    dto.LibraryId);
                var result = await mediator.Send(command);
                return Results.Created($"/api/task/{result.Id}", result);
            }
            catch (TaskAlreadyExistsException ex)
            {
                return Results.Conflict(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (LanguageNotFoundException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while creating task: {ex.Message}");
            }
        })
        .WithName("CreateTask")
        .WithSummary("Создание новой задачи")
        .WithDescription("Создание новую задачу программирования с заданными языками и уровнем сложности")
        .Produces<TaskDto>(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // Обновление задачи
        group.MapPut("/", async (UpdateTaskDto dto, IMediator mediator, HttpContext http) =>
        {
            try
            {
                ValidateAdminOrTeacherAccess(http);
                
                var command = new UpdateTaskCommand(
                    dto.Id, 
                    dto.Title, 
                    dto.Description,
                    dto.Difficulty, 
                    dto.Author,
                    dto.FunctionName, 
                    dto.PatternMain,
                    dto.PatternFunction, 
                    dto.Parameters,
                    dto.ReturnType,
                    dto.LanguageIds,
                    dto.LibrariesIds,
                    dto.TestCases);
                
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (TaskNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (LanguageNotFoundException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while updating task: {ex.Message}");
            }
        })
        .WithName("UpdateTask")
        .WithSummary("Обновление задачи")
        .WithDescription("Обновляет существующую задачу, включая связанные с ней языки программирования")
        .Produces<TaskDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);


        group.MapGet("/search-paged", async (IMediator mediator, [AsParameters] TaskTaskFilterPagedDto taskTaskFilter,
                HttpContext http) =>
        {
            try
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                var query = new SearchTasksPagedQuery(taskTaskFilter.SearchTerm, taskTaskFilter.Difficulty, taskTaskFilter.LanguageId,
                    taskTaskFilter.Page, taskTaskFilter.PageSize);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving tasks: {ex.Message}");
            }
        })
        .WithName("GetAllTasksPaged")
        .WithSummary("Получение списка задач с пагинацией")
        .WithDescription("Извлекает все задачи с дополнительной фильтрацией и разбивкой по страницам")
        .Produces<List<TaskDto>>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        
        group.MapGet("/search", async (IMediator mediator, [AsParameters] TaskFilterDto taskFilter, HttpContext http) =>
        {
            try
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                
                var query = new SearchTasksQuery(taskFilter.SearchTerm, taskFilter.Difficulty, taskFilter.LanguageId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving tasks: {ex.Message}");
            }
        })
        .WithName("GetAllTasks")
        .WithSummary("Получение списка задач")
        .WithDescription("Извлекает все задачи с дополнительной фильтрацией")
        .Produces<List<TaskDto>>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);;
        
        group.MapDelete("/{taskId:guid}", async (Guid taskId, IMediator mediator, HttpContext http) =>
            {
                try
                {
                ValidateAdminOrTeacherAccess(http);
                    
                    var command = new RemoveTaskCommand(taskId);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
                catch (TaskNotFoundException ex)
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
            .WithName("DeleterTasks")
            .WithSummary("Удаления задачи")
            .WithDescription("Удаляет задачи по его уникальному идентификатору")
            .Produces<List<TaskDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);;




        group.MapPost("/testCases/{taskId:guid}", async (Guid taskId, CreateTestCasesDto dto, IMediator mediator, HttpContext http) =>
        {
            try
            {
                ValidateAdminOrTeacherAccess(http);
                
                var command = new CreateTestCasesCommand(taskId, dto.TestCases);
                var result = await mediator.Send(command);
                return Results.Created($"/api/task/{taskId}/test-cases", result);
            }
            catch (TaskNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while creating test cases: {ex.Message}");
            }
        })
        .WithName("CreateTestCases")
        .WithSummary("Добавление тестов для задачи")
        .WithDescription("Добавляет набор тестовых случаев для проверки решений конкретной задачи программирования")
        .Produces<List<TestCaseDto>>(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        
        // Обновление тестового случая
        group.MapPut("/testCases/{testCaseId:guid}", async (Guid testCaseId, UpdateTestCaseDto dto, IMediator mediator,
                HttpContext http) =>
        {
            try
            {
                ValidateAdminOrTeacherAccess(http);
                
                var command = new UpdateTestsCaseCommand(testCaseId, dto.Input, dto.Output, dto.IsExample);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (TestCaseNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while updating test case: {ex.Message}");
            }
        })
        .WithName("UpdateTestCase")
        .WithSummary("Обновление тестового случая")
        .WithDescription("Обновляет входные данные или ожидаемый результат конкретного тестового случая")
        .Produces<TestCaseDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
        
        // Удаление тестового случая
        group.MapDelete("/testCases/{testCaseId:guid}", async (Guid testCaseId, IMediator mediator, HttpContext http) =>
        {
            try
            {
                ValidateAdminOrTeacherAccess(http);
                
                var command = new RemoveTestCasesCommand(testCaseId);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (TestCaseNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorResponse(ex.Message, "VALIDATION_ERROR"));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while deleting test case: {ex.Message}");
            }
        })
        .WithName("DeleteTestCase")
        .WithSummary("Удаление тестового случая")
        .WithDescription("Удаляет конкретный тестовый случай из задачи программирования")
        .Produces<DeleteResponseDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

       
        group.MapGet("/testCases/{taskId:guid}", async (Guid taskId, IMediator mediator, HttpContext http) =>
        {
            try
            {
                ValidateUserOrAdminOrTeacherAccess(http);
                
                var query = new GetTestCasesByTaskQuery(taskId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (TaskNotFoundException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message, ex.ErrorCode));
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while retrieving test cases: {ex.Message}");
            }
        })
        .WithName("GetTestCasesByTask")
        .WithSummary("Получение тестовых случаев задачи")
        .WithDescription("Извлекает все тестовые случаи для конкретной задачи программирования")
        .Produces<List<TestCaseDto>>(StatusCodes.Status200OK)
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


