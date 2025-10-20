namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class TaskLanguage:Entity
{
    public Guid IdTask { get; private set; }
    public Guid IdLanguage { get; private set; }
    public Task Task { get; private set; }
    public Language Language{ get; private set; }
}