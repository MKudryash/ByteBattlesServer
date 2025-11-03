using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;

// public class UserServiceClient: IUserServiceClient
// {
//     private readonly HttpClient _httpClient;
//     private readonly ILogger<UserServiceClient> _logger;
//     private readonly UserProfileServiceOptions _options;
//
//     public UserServiceClient(
//         HttpClient httpClient,
//         IOptions<UserProfileServiceOptions> options,
//         ILogger<UserServiceClient> logger)
//     {
//         _httpClient = httpClient;
//         _logger = logger;
//         _options = options.Value;
//
//         _httpClient.BaseAddress = new Uri(_options.BaseUrl);
//         _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
//     }
//     public Task<UserProfileDto> GetUserProfileAsync(Guid userId)
//     {
//         try
//         {
//             var user = _httpClient.GetAsync($"{_options.BaseUrl}/users/me");
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//
//     public Task UpdateUserStatsAsync(Guid userId, bool isSuccessful, TimeSpan executionTime, string taskDifficulty, Guid taskId)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<List<AchievementDto>> GetUserAchievementsAsync(Guid userId)
//     {
//         throw new NotImplementedException();
//     }
// }
// public class UserProfileServiceOptions
// {
//     public string BaseUrl { get; set; } = "http://localhost:5152";
//     public int TimeoutSeconds { get; set; } = 30;
// }