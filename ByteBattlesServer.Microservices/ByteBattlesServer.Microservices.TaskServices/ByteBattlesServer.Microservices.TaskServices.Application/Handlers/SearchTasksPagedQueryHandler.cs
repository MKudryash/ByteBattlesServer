using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
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
        
        return tasks.Select(x=>TaskMapping.MapToDtoAllInfo(x)).ToList();
    }
}