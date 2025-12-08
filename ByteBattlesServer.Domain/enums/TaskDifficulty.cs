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
    
    // Для обратного преобразования создаем словарь в обратную сторону
    private static readonly Dictionary<string, TaskDifficulty> StringToDifficulty = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Easy", TaskDifficulty.Easy },
        { "Middle", TaskDifficulty.Medium },
        { "Hard", TaskDifficulty.Hard },
        // Можно добавить варианты на русском или другие синонимы
        { "Легкая", TaskDifficulty.Easy },
        { "Средняя", TaskDifficulty.Medium },
        { "Сложная", TaskDifficulty.Hard },
        { "Easy", TaskDifficulty.Easy },
        { "Medium", TaskDifficulty.Medium }, // Если кто-то использует английское название
        { "Hard", TaskDifficulty.Hard }
    };
    
    public static string GetName(TaskDifficulty difficulty)
    {
        return DifficultyNames.TryGetValue(difficulty, out var name) 
            ? name 
            : difficulty.ToString();
    }
    
    public static TaskDifficulty? FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
            
        return StringToDifficulty.TryGetValue(name, out var difficulty)
            ? difficulty
            : null;
    }
    
    public static TaskDifficulty FromNameOrDefault(string name, TaskDifficulty defaultValue = TaskDifficulty.Easy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return defaultValue;
            
        return StringToDifficulty.TryGetValue(name, out var difficulty)
            ? difficulty
            : defaultValue;
    }
    
    // Дополнительно: метод для проверки корректности
    public static bool IsValidName(string name)
    {
        return !string.IsNullOrWhiteSpace(name) && 
               StringToDifficulty.ContainsKey(name);
    }
    
    // Метод для получения всех доступных вариантов
    public static Dictionary<string, string> GetAllOptions()
    {
        return DifficultyNames.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => kvp.Value
        );
    }
}