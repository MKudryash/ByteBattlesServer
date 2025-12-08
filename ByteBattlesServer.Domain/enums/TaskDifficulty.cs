namespace ByteBattlesServer.Domain.enums;

public enum TaskDifficulty
{
    Easy =0,
    Medium =1,
    Hard =2
}

public static class TaskDifficultyHelper
{
    private static readonly Dictionary<TaskDifficulty, string> DifficultyNames;
    private static readonly Dictionary<string, TaskDifficulty> StringToDifficulty;
    
    // Статический конструктор для безопасной инициализации
    static TaskDifficultyHelper()
    {
        try
        {
            // Инициализируем первый словарь
            DifficultyNames = new Dictionary<TaskDifficulty, string>
            {
                { TaskDifficulty.Easy, "Easy" },
                { TaskDifficulty.Medium, "Middle" },
                { TaskDifficulty.Hard, "Hard" }
            };
            
            // Инициализируем обратный словарь
            StringToDifficulty = new Dictionary<string, TaskDifficulty>(StringComparer.OrdinalIgnoreCase);
            
            // Заполняем обратный словарь из первого
            foreach (var kvp in DifficultyNames)
            {
                StringToDifficulty[kvp.Value] = kvp.Key;
                
                // Также добавляем строковое представление enum
                StringToDifficulty[kvp.Key.ToString()] = kvp.Key;
            }
            
            // Добавляем русские варианты (опционально)
            StringToDifficulty["Легкая"] = TaskDifficulty.Easy;
            StringToDifficulty["Средняя"] = TaskDifficulty.Medium;
            StringToDifficulty["Сложная"] = TaskDifficulty.Hard;
        }
        catch (Exception ex)
        {
            // Логируем ошибку для отладки
            Console.WriteLine($"Error initializing TaskDifficultyHelper: {ex.Message}");
            throw;
        }
    }
    
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