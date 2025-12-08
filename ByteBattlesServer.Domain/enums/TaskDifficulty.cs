namespace ByteBattlesServer.Domain.enums;

public enum TaskDifficulty
{
    Easy ,
    Medium,
    Hard
}
public static class TaskDifficultyHelper
{
    private static readonly Dictionary<TaskDifficulty, string> DifficultyNames = new()
    {
        { TaskDifficulty.Easy, "Easy" },
        { TaskDifficulty.Medium, "Middle" },
        { TaskDifficulty.Hard, "Hard" }
    };
    
    public static string GetName(TaskDifficulty difficulty)
    {
        return DifficultyNames.TryGetValue(difficulty, out var name) 
            ? name 
            : difficulty.ToString();
    }
}