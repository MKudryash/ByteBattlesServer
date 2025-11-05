using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class UserStats : ValueObject
{
    public int TotalProblemsSolved { get; private set; }
    public int TotalBattles { get; private set; }
    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public int Draws { get; private set; }
    public int CurrentStreak { get; private set; }
    public int MaxStreak { get; private set; }
    public int TotalExperience { get; private set; }
    
    // Новая статистика по задачам
    public int EasyProblemsSolved { get; private set; }
    public int MediumProblemsSolved { get; private set; }
    public int HardProblemsSolved { get; private set; }
    public int TotalSubmissions { get; private set; }
    public int SuccessfulSubmissions { get; private set; }
    public TimeSpan TotalExecutionTime { get; private set; }
    public HashSet<Guid> SolvedTaskIds { get; private set; } = new();
    
    // Вычисляемые свойства
    public double WinRate => TotalBattles > 0 ? (double)Wins / TotalBattles * 100 : 0;
    public double SuccessRate => TotalSubmissions > 0 ? (double)SuccessfulSubmissions / TotalSubmissions * 100 : 0;
    public TimeSpan AverageExecutionTime => SuccessfulSubmissions > 0 ? TimeSpan.FromTicks(TotalExecutionTime.Ticks / SuccessfulSubmissions) : TimeSpan.Zero;

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

            // Учитываем задачу только если она решена впервые
            if (SolvedTaskIds.Add(taskId))
            {
                TotalProblemsSolved++;
                
                // Увеличиваем счетчик по сложности
                switch (difficulty)
                {
                    case TaskDifficulty.Easy:
                        EasyProblemsSolved++;
                        TotalExperience += 10; // Опыт за легкие задачи
                        break;
                    case TaskDifficulty.Medium:
                        MediumProblemsSolved++;
                        TotalExperience += 25; // Опыт за средние задачи
                        break;
                    case TaskDifficulty.Hard:
                        HardProblemsSolved++;
                        TotalExperience += 50; // Опыт за сложные задачи
                        break;
                }
            }
        }
        else
        {
            // Сбрасываем серию при неудачной попытке
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