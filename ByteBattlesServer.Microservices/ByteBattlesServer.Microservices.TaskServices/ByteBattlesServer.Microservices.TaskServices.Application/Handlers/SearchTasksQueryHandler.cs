using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class SearchTasksQueryHandler: IRequestHandler<SearchTasksQuery, List<TaskDto>>
{
    private readonly ITaskRepository _repository;
    
    public SearchTasksQueryHandler(ITaskRepository repository)=> _repository = repository;
    
    public async Task<List<TaskDto>> Handle(SearchTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _repository.SearchTask(request.Difficulty, request.LanguageId, request.SearchTerm);
        
        return tasks.Select(x=>MapToDto(x)).ToList();
    }
    private TaskDto MapToDto(Domain.Entities.Task task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Difficulty = task.Difficulty.ToString(),
        Author = task.Author,
        FunctionName = task.FunctionName,
        InputParameters = task.InputParameters,
        OutputParameters = task.OutputParameters,
        TaskLanguages = task.TaskLanguages.Select(tl => new TaskLanguageDto()
        {
            LanguageId = tl.Id,
            LanguageTitle = tl.Language.Title,
            LanguageShortTitle = tl.Language.ShortTitle,
        }).ToList()
    };
}


public class SearchLanguageQueryHandler: IRequestHandler<SearchLanguagesQuery, List<LanguageDto>>
{
    private readonly ILanguageRepository _repository;
    
    public SearchLanguageQueryHandler(ILanguageRepository repository)=> _repository = repository;
    
    public async Task<List<LanguageDto>> Handle(SearchLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languages = await _repository.SearchLanguages(request.SearchTerm);
        
        return languages.Select(x=>MapToDto(x)).ToList();
    }
    private LanguageDto MapToDto(Language language) => new()
    {
        Id = language.Id,
        Title = language.Title,
        ShortTitle = language.ShortTitle
    };
}

public class SearchLanguagePagedQueryHandler: IRequestHandler<SearchLanguagesPagedQuery, List<LanguageDto>>
{
    private readonly ILanguageRepository _repository;
    
    public SearchLanguagePagedQueryHandler(ILanguageRepository repository)=> _repository = repository;
    
    public async Task<List<LanguageDto>> Handle(SearchLanguagesPagedQuery request, CancellationToken cancellationToken)
    {
        var languages = await _repository.SearchLanguagesPagedAsync(request.SearchTerm
        ,request.Page, request.PageSize);
        
        return languages.Select(x=>MapToDto(x)).ToList();
    }
    private LanguageDto MapToDto(Language language) => new()
    {
        Id = language.Id,
        Title = language.Title,
        ShortTitle = language.ShortTitle
    };
}

