using System.Text.Json;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;

public class TaskServiceClient : ITaskServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TaskServiceClient> _logger;
    private readonly TaskServiceOptions _options;

    public TaskServiceClient(
        HttpClient httpClient,
        IOptions<TaskServiceOptions> options,
        ILogger<TaskServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<TaskDto> GetTaskAsync(Guid taskId)
    {
        try
        {
            _logger.LogInformation("Getting task with ID: {TaskId}", taskId);

            var response = await _httpClient.GetAsync($"/api/task/{taskId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get task {TaskId}. Status: {StatusCode}", taskId, response.StatusCode);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new TaskNotFoundException($"Task with ID {taskId} not found");
                }
                
                throw new HttpRequestException($"Failed to get task: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var task = JsonSerializer.Deserialize<TaskDto>(
                content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (task == null)
            {
                throw new InvalidOperationException("Failed to deserialize task response");
            }

            _logger.LogInformation("Successfully retrieved task: {TaskTitle}", task.Title);
            return task;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while getting task {TaskId}", taskId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<List<TestCaseDto>> GetTestCasesAsync(Guid taskId)
    {
        try
        {
            _logger.LogInformation("Getting test cases for task ID: {TaskId}", taskId);

            var response = await _httpClient.GetAsync($"/api/task/testCases/{taskId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get test cases for task {TaskId}. Status: {StatusCode}", 
                    taskId, response.StatusCode);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new TaskNotFoundException($"Task with ID {taskId} not found");
                }
                
                throw new HttpRequestException($"Failed to get test cases: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var testCases = JsonSerializer.Deserialize<List<TestCaseDto>>(
                content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (testCases == null)
            {
                _logger.LogWarning("No test cases found for task {TaskId}", taskId);
                return new List<TestCaseDto>();
            }

            _logger.LogInformation("Successfully retrieved {Count} test cases for task {TaskId}", 
                testCases.Count, taskId);
            
            return testCases;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while getting test cases for task {TaskId}", taskId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting test cases for task {TaskId}", taskId);
            throw;
        }
    }
}

// Configuration options
public class TaskServiceOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5276";
    public int TimeoutSeconds { get; set; } = 30;
}


// Custom exceptions
public class TaskNotFoundException : Exception
{
    public TaskNotFoundException(string message) : base(message) { }
}

public class TestCaseNotFoundException : Exception
{
    public TestCaseNotFoundException(string message) : base(message) { }
}