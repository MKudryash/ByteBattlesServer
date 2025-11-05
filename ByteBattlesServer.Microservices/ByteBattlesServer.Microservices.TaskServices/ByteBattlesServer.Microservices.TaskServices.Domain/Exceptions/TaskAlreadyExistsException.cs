using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;

public class TaskAlreadyExistsException:TaskException
{
    public TaskAlreadyExistsException(Guid userId) 
        : base($"Task already exists for task ID: {userId}", "TASK_ALREADY_EXISTS")
    {
    } 
    public TaskAlreadyExistsException(string title) 
        : base($"Task already exists for task title: {title}", "TASK_ALREADY_EXISTS")
    {
    }
}