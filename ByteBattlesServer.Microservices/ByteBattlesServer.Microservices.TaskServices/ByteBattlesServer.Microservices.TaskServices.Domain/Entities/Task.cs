using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class Task:Entity
{
    public string? Title { get; set; }

    public string? Description { get; set; }
    
    public Difficulty Difficulty { get; private set; }

    public string? Author { get; private set; }

    public string? FunctionName { get; private set; }

    public string? InputParameters { get; private set; }

    public string? OutputParameters { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public virtual ICollection<TaskLanguage> TaskLanguages { get; private set; } = new List<TaskLanguage>();
    public virtual ICollection<TestCases> TestCases { get; private set; } = new List<TestCases>();
    private Task() { }

    public Task(string title, string description, string difficulty, string author, string functionName,
        string inputParameters, string outputParameters)
    {
        Title = title;
        Description = description;
        Difficulty = Enums.Difficulty.Easy;
        Author = author;
        FunctionName = functionName;
        InputParameters = inputParameters;
        OutputParameters = outputParameters;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateTask(string? title= null,
        string? description= null,
        string? difficulty = null, 
        string? author = null,
        string? functionName = null,
        string? inputParameters = null,
        string? outputParameters = null)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title =title.Trim();
        if (!string.IsNullOrWhiteSpace(description))
            Description =description.Trim();
        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            if (Enum.TryParse<Difficulty>(difficulty.Trim(), true, out var parsedDifficulty))
            {
                Difficulty = parsedDifficulty;
            }
           throw new ArgumentException($"Invalid difficulty {difficulty}");
        }
        if (!string.IsNullOrWhiteSpace(author))
            Author = author.Trim();
        
        if (!string.IsNullOrWhiteSpace(functionName))
            FunctionName = functionName.Trim();
        
        if (!string.IsNullOrWhiteSpace(inputParameters))
            InputParameters = inputParameters.Trim();
        
        if (!string.IsNullOrWhiteSpace(outputParameters))
            OutputParameters = outputParameters.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AddTestCase(string input, string expectedOutput)
    {
        var testCase = new TestCases
        {
            TaskId = Id,
            Input = input,
            ExpectedOutput = expectedOutput,
            CreatedAd = DateTime.UtcNow,
            UpdatedAd = DateTime.UtcNow
        };
        
        TestCases.Add(testCase);
        UpdatedAt = DateTime.UtcNow;
    }
    public void RemoveTestCase(TestCases testCase)
    {
        TestCases.Remove(testCase);
        UpdatedAt = DateTime.UtcNow;
    }


    public void UpdateDate()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTestCases(IEnumerable<(string Input, string ExpectedOutput)> testCases)
    {
        foreach (var (input, expectedOutput) in testCases)
        {
            AddTestCase(input, expectedOutput);
        }
    }

}