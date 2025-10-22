namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class TestCases: Entity
{

    public Guid TaskId { get; set; }

    public string Input { get; set; } = null!;

    public string ExpectedOutput { get; set; } = null!;

    public DateTime CreatedAd { get; set; }
    
    public DateTime UpdatedAd { get; set; }
    
    public Task Task  { get; set; } = null!;
    public TestCases() { }

    public TestCases(Guid taskId, string input, string expectedOutput)
    {
        TaskId = taskId;
        Input = input;
        ExpectedOutput = expectedOutput;
        CreatedAd = DateTime.UtcNow;
        UpdatedAd = DateTime.UtcNow;
    }

    public void Update(string newInput, string newExpectedOutput)
    {
        Input = newInput;
        ExpectedOutput = newExpectedOutput;
        UpdatedAd = DateTime.UtcNow;
    }
}