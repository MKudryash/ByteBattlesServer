using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class Task:Entity
{
    public string? Title { get; set; }

    public string? Description { get; set; }
    
    public Difficulty Difficulty { get; private set; }

    public string? Author { get; private set; }

    public string? FunctionName { get; private set; }

    public string? PatternFunction { get; private set; }

    public string? PatternMain { get; private set; }
    
    public string? Parameters { get; private set; }
    public string? ReturnType { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
    
    public int TotalAttempts { get; private set; }
    public int SuccessfulAttempts { get; private set; }
    public double SuccessRate => TotalAttempts > 0 ? (double)SuccessfulAttempts / TotalAttempts * 100 : 0;
    public double AverageExecutionTime { get; private set; }
    

    public virtual ICollection<TaskLanguage> TaskLanguages { get; private set; } = new List<TaskLanguage>();
    public virtual ICollection<TestCases> TestCases { get; private set; } = new List<TestCases>();
    private Task() { }

    public Task(string title, string description, string difficulty, string author, string functionName,
        string patternMain, string patternFunction, string parameters,  string returnType)
    {
        Title = title;
        Description = description;
        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            if (Enum.TryParse<Difficulty>(difficulty.Trim(), true, out var parsedDifficulty))
            {
                Difficulty = parsedDifficulty;
            }
            else
            {
                Difficulty = Difficulty.Easy;
            }
        }
        Author = author;
        FunctionName = functionName;
        PatternFunction = patternFunction;
        PatternMain = patternMain;
        Parameters = parameters;
        ReturnType = returnType;
        CreatedAt = DateTime.UtcNow;
        
    }

    public void UpdateTask(string? title= null,
        string? description= null,
        string? difficulty = null, 
        string? author = null,
        string? functionName = null,
        string? patternMain = null,
        string? patternFunction = null,
        string? patternParameters = null,
        string? returnType = null)
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
            else
            {
                throw new ArgumentException($"Invalid difficulty {difficulty}");
            }
        }
        if (!string.IsNullOrWhiteSpace(author))
            Author = author.Trim();
        
        if (!string.IsNullOrWhiteSpace(functionName))
            FunctionName = functionName.Trim();
        
        if (!string.IsNullOrWhiteSpace(patternMain))
            PatternMain = patternMain.Trim();
        
        if (!string.IsNullOrWhiteSpace(patternFunction))
            PatternFunction = patternFunction.Trim();
        if (!string.IsNullOrWhiteSpace(patternParameters))
            Parameters = patternParameters.Trim();
        if (!string.IsNullOrWhiteSpace(returnType))
            ReturnType = returnType.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
    



    public void UpdateDate()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void RecordAttempt(bool isSuccessful, TimeSpan executionTime)
    {
        TotalAttempts++;
        if (isSuccessful)
        {
            SuccessfulAttempts++;
        }
        
        AverageExecutionTime = ((AverageExecutionTime * (TotalAttempts - 1)) + executionTime.TotalMilliseconds) / TotalAttempts;
        UpdatedAt = DateTime.UtcNow;
    }

}