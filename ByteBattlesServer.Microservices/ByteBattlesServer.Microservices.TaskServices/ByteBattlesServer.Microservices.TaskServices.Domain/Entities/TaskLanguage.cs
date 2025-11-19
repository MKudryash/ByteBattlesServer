namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class TaskLanguage:Entity
{
    public Guid IdTask { get; private set; }
    public Guid IdLanguage { get; private set; }
    public Task Task { get; private set; }
    public Language Language{ get; private set; }
    public TaskLanguage(Guid idTask, Guid idLanguage)
    {
        IdTask = idTask;
        IdLanguage = idLanguage;
    }
}

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