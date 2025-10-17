using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class UserStats: ValueObject
{
    public int TotalProblemsSolved { get; private set; }
    public int TotalBattles { get; private set; }
    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public int Draws { get; private set; }
    public int CurrentStreak { get; private set; }
    public int MaxStreak { get; private set; }
    public int TotalExperience { get; private set; }
    public double WinRate => TotalBattles > 0 ? (double)Wins / TotalBattles * 100 : 0;

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
    }
}