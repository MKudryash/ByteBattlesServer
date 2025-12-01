using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;

namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class TaskFilterDto
{
    public string? SearchTerm { get; set; } = null;
    public TaskDifficulty? Difficulty { get; set; } = null;
    public Guid? LanguageId { get; set; } = null;
}

public class LanguageFilerDto
{
    public string? SearchTerm { get; set; } = null;
}