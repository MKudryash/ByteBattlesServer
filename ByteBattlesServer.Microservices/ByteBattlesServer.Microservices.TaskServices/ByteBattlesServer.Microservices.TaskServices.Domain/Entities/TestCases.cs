namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class TestCases: Entity
{

    public Guid TaskId { get; set; }

    public string Input { get; set; } = null!;

    public string ExpectedOutput { get; set; } = null!;
    
    public bool IsExample { get; set; } = false;

    public DateTime CreatedAd { get; set; }
    
    public DateTime UpdatedAd { get; set; }
    
    public Task Task  { get; set; } = null!;
    public TestCases() { }

    public TestCases(Guid taskId, string input, string expectedOutput, bool isExample)
    {
        TaskId = taskId;
        Input = input;
        ExpectedOutput = expectedOutput;
        IsExample = isExample;
        CreatedAd = DateTime.UtcNow;
        UpdatedAd = DateTime.UtcNow;
    }

    public void Update(string? newInput, string? newExpectedOutput, bool isExample)
    {
        if (!string.IsNullOrWhiteSpace(Input))
            Input =newInput.Trim();
        if (!string.IsNullOrWhiteSpace(ExpectedOutput))
            ExpectedOutput = newExpectedOutput.Trim();
        
        IsExample = isExample;
        
        UpdatedAd = DateTime.UtcNow;
    }
}