using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class SearchTasksPagedQueryHandler: IRequestHandler<SearchTasksPagedQuery, List<TaskDto>>
{
    private readonly ITaskRepository _repository;
    
    public SearchTasksPagedQueryHandler(ITaskRepository repository)=> _repository = repository;
    
    public async Task<List<TaskDto>> Handle(SearchTasksPagedQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _repository.SearchTasksPagedAsync(request.Difficulty, request.LanguageId,
            request.SearchTerm, 
            request.Page, 
            request.PageSize);
        
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
        PatternMain = task.PatternMain,
        PatternFuction = task.PatternFunction,
        TaskLanguages = task.TaskLanguages.Select(tl => new TaskLanguageDto()
        {
            LanguageId = tl.Id,
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