using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;
using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class CreateTaskCommandHandler:IRequestHandler<CreateTaskCommand, TaskDto>
{
    
    private readonly ITaskRepository _taskRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, ILanguageRepository languageRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
       var existingTask = await _taskRepository.GetByTitleAsync(request.Title);
        if (existingTask != null)
            throw new TaskAlreadyExistsException(request.Title);
        
        var languages = new List<Language>();
        foreach (var languageId in request.LanguageIds)
        {
            var language = await _languageRepository.GetByIdAsync(languageId);
            if (language == null)
                throw new LanguageNotFoundException(languageId);
            languages.Add(language);
        }
        
        
        var task = new Domain.Entities.Task(request.Title, request.Description,
            request.Difficulty, request.Author,request.FunctionName,request.PatternMain,request.PatternFunction);
       
        
        
        foreach (var language in languages)
        {
            var taskLanguage = new TaskLanguage(task.Id, language.Id);
            task.TaskLanguages.Add(taskLanguage);
        }

        
        await _taskRepository.AddAsync(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

    
        var createdTask = await _taskRepository.GetByIdAsync(task.Id);
        return MapToDto(createdTask);

    }

    private TaskDto MapToDto(Task task) => new()
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
            }).ToList()
    };
}