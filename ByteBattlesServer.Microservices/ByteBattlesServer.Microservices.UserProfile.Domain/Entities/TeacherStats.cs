namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class TeacherStats : ValueObject
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserProfileId { get; private set; } // Явный FK
    public UserProfile UserProfile { get; private set; }
    public int CreatedTasks { get; private set; }
    public int ActiveStudents { get; private set; }
    public double AverageRating { get; private set; }
    public int TotalSubmissions { get; private set; }

    public TeacherStats()
    {
        CreatedTasks = 0;
        ActiveStudents = 0;
        AverageRating = 0;
        TotalSubmissions = 0;
    }

    public void UpdateTeacherStats(int? createdTasks = null, int? activeStudents = null, 
        double? averageRating = null, int? totalSubmissions = null)
    {
        if (createdTasks.HasValue)
            CreatedTasks = createdTasks.Value;
            
        if (activeStudents.HasValue)
            ActiveStudents = activeStudents.Value;
            
        if (averageRating.HasValue)
            AverageRating = averageRating.Value;
            
        if (totalSubmissions.HasValue)
            TotalSubmissions = totalSubmissions.Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CreatedTasks;
        yield return ActiveStudents;
        yield return AverageRating;
        yield return TotalSubmissions;
    }
}