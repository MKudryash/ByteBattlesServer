using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;

namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class TaskInfoRequest
{
    public Guid Id { get; set; }
    public Difficulty Difficulty { get; set; }
    public Guid TaskId { get; set; }
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}