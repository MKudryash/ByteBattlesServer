using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Mapping;


public class TaskMapping
{
    public static TaskDto MapToDtoAllInfo(Task task)
    {
        if (task == null) return null;
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Difficulty = task.Difficulty.ToString(),
            Author = task.Author,
            FunctionName = task.FunctionName,
            PatternMain = task.PatternMain,
            PatternFunction = task.PatternFunction,
            SuccessRate = task.SuccessRate,
            AverageExecutionTime = task.AverageExecutionTime,
            SuccessfulAttempts = task.SuccessfulAttempts,
            TotalAttempts = task.TotalAttempts,
            Parameters =  task.Parameters,
            ReturnType = task.ReturnType,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            Libraries = task.Libraries.Select(l=>new LibraryDto()
            {
                Id = l.IdLibrary,
                Name = l.Library.NameLibrary,
                Version = l.Library.Version,
                Description = l.Library.Description,
                languageId = l.Library.LanguageId
            }).ToList(),
            TaskLanguages = task.TaskLanguages.Select(tl => new TaskLanguageDto()
            {
                LanguageId = tl.IdLanguage,
                LanguageTitle = tl.Language.Title,
                LanguageShortTitle = tl.Language.ShortTitle,
            }).ToList(),
            TestCases = task.TestCases.Select(t => new TestCaseDto()
            {
                Id = t.Id,
                Input = t.Input,
                Output = t.ExpectedOutput
            }).ToList()
        };
    }
    public static TaskDto MapToDto(Task task)
    {
        if (task == null) return null;
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Difficulty = task.Difficulty.ToString(),
            Author = task.Author,
            FunctionName = task.FunctionName,
            PatternMain = task.PatternMain,
            PatternFunction = task.PatternFunction,
            SuccessRate = task.SuccessRate,
            AverageExecutionTime = task.AverageExecutionTime,
            SuccessfulAttempts = task.SuccessfulAttempts,
            TotalAttempts = task.TotalAttempts,
            Parameters =  task.Parameters,
            ReturnType = task.ReturnType,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            
            Libraries = task.Libraries.Select(l=>new LibraryDto()
            {
                Id = l.IdLibrary,
                Name = l.Library.NameLibrary,
                Version = l.Library.Version,
                Description = l.Library.Description,
                languageId = l.Library.LanguageId
            }).ToList(),
            TaskLanguages = task.TaskLanguages.Select(tl => new TaskLanguageDto()
            {
                LanguageId = tl.IdLanguage,
                LanguageTitle = tl.Language.Title,
                LanguageShortTitle = tl.Language.ShortTitle,
            }).ToList()
        };
    }
}