namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class TaskLibrary : Entity
{
    public Guid IdTask { get; private set; }
    public Guid IdLibrary { get; private set; }
    public Task Task { get; private set; }
    public Library Library{ get; private set; }
    public TaskLibrary(Guid idTask, Guid idLibrary)
    {
        IdTask = idTask;
        IdLibrary = idLibrary;
    }
}