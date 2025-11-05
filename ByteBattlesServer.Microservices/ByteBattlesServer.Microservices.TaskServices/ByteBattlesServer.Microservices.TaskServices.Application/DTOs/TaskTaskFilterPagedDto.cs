namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class TaskTaskFilterPagedDto:TaskFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TaskLanguageFilterPagedDto:LanguageFilerDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}