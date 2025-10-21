using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;

public class TaskNotFoundException:TaskException

{
    public TaskNotFoundException(Guid taskId) 
        : base($"Task not found for task ID: {taskId}", "TASK_NOT_FOUND")
    {
    }
    public TaskNotFoundException(string title) 
        : base($"User task not found for title: {title}", "TASK_NOT_FOUND")
    {
    }
}
public class LanguageNotFoundException:LanguageException

{
    public LanguageNotFoundException(Guid languageId) 
        : base($"Task not found for languageId ID: {languageId}", "TASK_NOT_FOUND")
    {
    }
    public LanguageNotFoundException(string title) 
        : base($"User language not found for title: {title}", "TASK_NOT_FOUND")
    {
    }
}