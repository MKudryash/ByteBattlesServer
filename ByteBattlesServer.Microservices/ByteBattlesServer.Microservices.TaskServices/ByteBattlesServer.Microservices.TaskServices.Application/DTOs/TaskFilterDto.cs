namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class TaskFilterDto
{
    private string? SearchTerm { get; set; } = null;
    private string? Difficulty { get; set; } = null;
    private Guid? LanguageId { get; set; } = null;
        private int Page { get; set; } = 1;
        private int PageSize { get; set; } = 20;
}