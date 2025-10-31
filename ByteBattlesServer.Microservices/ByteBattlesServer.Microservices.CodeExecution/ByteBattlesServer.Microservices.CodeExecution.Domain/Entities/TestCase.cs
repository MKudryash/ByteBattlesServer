namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;

public class TestCase
{
    public string Input { get; }
    public string ExpectedOutput { get; }
        
    public TestCase(string input, string expectedOutput)
    {
        Input = input ?? throw new ArgumentNullException(nameof(input));
        ExpectedOutput = expectedOutput ?? throw new ArgumentNullException(nameof(expectedOutput));
    }
}