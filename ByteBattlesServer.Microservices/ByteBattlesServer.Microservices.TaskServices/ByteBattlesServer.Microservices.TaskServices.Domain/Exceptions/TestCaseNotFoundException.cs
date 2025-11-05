namespace ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;

public class TestCaseNotFoundException:TestCaseException

{
    public TestCaseNotFoundException(Guid taskId) 
        : base($"Task not found for test  case ID: {taskId}", "TEST_CASE_NOT_FOUND")
    {
    }
}