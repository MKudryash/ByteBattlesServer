namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record TaskCountDifficatly()
{
    public int Easy { get; init; }
    public int Medium { get; init; }
    public int Hard { get; init; }
}