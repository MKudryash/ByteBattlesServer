using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
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
        
        return tasks.Select(x=>TaskMapping.MapToDtoAllInfo(x)).ToList();
    }
}