using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;


public class UserStats : ValueObject
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserProfileId { get; private set; } // Явный FK
    public UserProfile UserProfile { get; private set; }
        /// <summary>
    /// Общее количество решенных задач пользователем
    /// </summary>
    public int TotalProblemsSolved { get; private set; }
    
    /// <summary>
    /// Общее количество проведенных баттлов (соревновательных матчей)
    /// </summary>
    public int TotalBattles { get; private set; }
    
    /// <summary>
    /// Количество побед в баттлах
    /// </summary>
    public int Wins { get; private set; }
    
    /// <summary>
    /// Количество поражений в баттлах
    /// </summary>
    public int Losses { get; private set; }
    
    /// <summary>
    /// Количество ничьих в баттлах
    /// </summary>
    public int Draws { get; private set; }
    
    /// <summary>
    /// Текущая серия побед подряд
    /// </summary>
    public int CurrentStreak { get; private set; }
    
    /// <summary>
    /// Максимальная достигнутая серия побед подряд
    /// </summary>
    public int MaxStreak { get; private set; }
    
    /// <summary>
    /// Общее количество опыта пользователя
    /// </summary>
    public int TotalExperience { get; private set; }
    
    // Статистика по сложности решенных задач
    
    /// <summary>
    /// Количество решенных задач легкой сложности
    /// </summary>
    public int EasyProblemsSolved { get; private set; }
    
    /// <summary>
    /// Количество решенных задач средней сложности
    /// </summary>
    public int MediumProblemsSolved { get; private set; }
    
    /// <summary>
    /// Количество решенных задач высокой сложности
    /// </summary>
    public int HardProblemsSolved { get; private set; }
    
    /// <summary>
    /// Общее количество отправленных решений (попыток)
    /// </summary>
    public int TotalSubmissions { get; private set; }
    
    /// <summary>
    /// Количество успешных отправок решений
    /// </summary>
    public int SuccessfulSubmissions { get; private set; }
    
    /// <summary>
    /// Общее время выполнения всех решений
    /// </summary>
    public TimeSpan TotalExecutionTime { get; private set; }
    
    /// <summary>
    /// Множество идентификаторов решенных задач (для отслеживания уникальных решений)
    /// </summary>
    public HashSet<Guid> SolvedTaskIds { get; set; } = new();
    
    /// <summary>
    /// Процент побед в баттлах
    /// Вычисляется как: (Количество побед / Общее количество баттлов) * 100%
    /// Возвращает 0 если пользователь не участвовал в баттлах
    /// </summary>
    /// <example>
    /// Если Wins = 15, TotalBattles = 20, то WinRate = 75.0
    /// </example>
    public double WinRate => TotalBattles > 0 ? (double)Wins / TotalBattles * 100 : 0;
    
    /// <summary>
    /// Процент успешных отправок решений
    /// Вычисляется как: (Успешные отправки / Все отправки) * 100%
    /// Показывает эффективность пользователя в решении задач
    /// Возвращает 0 если не было отправок
    /// </summary>
    /// <example>
    /// Если SuccessfulSubmissions = 45, TotalSubmissions = 60, то SuccessRate = 75.0
    /// </example>
    public double SuccessRate => TotalSubmissions > 0 ? (double)SuccessfulSubmissions / TotalSubmissions * 100 : 0;
    
    /// <summary>
    /// Среднее время выполнения успешных решений
    /// Вычисляется как: Общее время выполнения / Количество успешных отправок
    /// Показывает среднюю скорость написания рабочего кода
    /// Возвращает TimeSpan.Zero если не было успешных отправок
    /// </summary>
    /// <example>
    /// Если TotalExecutionTime = 5 минут, SuccessfulSubmissions = 10, 
    /// то AverageExecutionTime = 30 секунд на решение
    /// </example>
    public TimeSpan AverageExecutionTime => SuccessfulSubmissions > 0 
        ? TimeSpan.FromTicks(TotalExecutionTime.Ticks / SuccessfulSubmissions) 
        : TimeSpan.Zero;
    public UserStats() { }

    public void UpdateStats(BattleResult battleResult)
    {
        TotalBattles++;
        TotalExperience += battleResult.ExperienceGained;

        switch (battleResult.Result)
        {
            case BattleResultType.Win:
                Wins++;
                CurrentStreak++;
                MaxStreak = Math.Max(MaxStreak, CurrentStreak);
                break;
            case BattleResultType.Loss:
                Losses++;
                CurrentStreak = 0;
                break;
            case BattleResultType.Draw:
                Draws++;
                CurrentStreak = 0;
                break;
        }
    }

    public void UpdateProblemStats(bool isSuccessful, 
        TaskDifficulty difficulty, 
        TimeSpan executionTime, Guid taskId)
    {
        TotalSubmissions++;
        
        if (isSuccessful)
        {
            SuccessfulSubmissions++;
            TotalExecutionTime += executionTime;
            
            if (SolvedTaskIds.Add(taskId))
            {
                TotalProblemsSolved++;
                
                switch (difficulty)
                {
                    case TaskDifficulty.Easy:
                        EasyProblemsSolved++;
                        TotalExperience += 10; 
                        break;
                    case TaskDifficulty.Medium:
                        MediumProblemsSolved++;
                        TotalExperience += 25;
                        break;
                    case TaskDifficulty.Hard:
                        HardProblemsSolved++;
                        TotalExperience += 50; 
                        break;
                }
            }
        }
        else
        {
            CurrentStreak = 0;
        }
    }

    public bool HasSolvedTask(Guid taskId) => SolvedTaskIds.Contains(taskId);

    public void IncrementProblemsSolved(int experience)
    {
        TotalProblemsSolved++;
        TotalExperience += experience;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalProblemsSolved;
        yield return TotalBattles;
        yield return Wins;
        yield return Losses;
        yield return Draws;
        yield return CurrentStreak;
        yield return MaxStreak;
        yield return TotalExperience;
        yield return EasyProblemsSolved;
        yield return MediumProblemsSolved;
        yield return HardProblemsSolved;
        yield return TotalSubmissions;
        yield return SuccessfulSubmissions;
    }
}