using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _repository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateTaskCommandHandler(
        ITaskRepository repository,  
        IUnitOfWork unitOfWork, 
        ILanguageRepository languageRepository)
    {
        _repository = repository;
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        // Получаем задачу с включенными языками
        var task = await _repository.GetByIdAsync(request.TaskId);
        
        if (task == null)
            throw new TaskNotFoundException(request.TaskId);
        
        // Обновляем основные поля задачи
        task.UpdateTask(
            request.Title, 
            request.Description,
            request.Difficulty,
            request.Author, 
            request.FunctionName,
            request.PatternMain, 
            request.PatternFunction,
            request.Parameters,
            request.ReturnType);
        
        // Обновляем языки если переданы
        if (request.LanguageIds != null && request.LanguageIds.Any())
        {
            await UpdateTaskLanguagesAsync(task, request.LanguageIds);
        }
        
        // Явно отмечаем задачу как измененную
        _repository.Update(task);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Перезагружаем задачу с актуальными данными
        var updatedTask = await _repository.GetByIdAsync(request.TaskId);
        return TaskMapping.MapToDto(updatedTask);
    }
    
    private async Task UpdateTaskLanguagesAsync(Domain.Entities.Task task, List<Guid> newLanguageIds)
    {
        // Валидируем новые языки
        var validLanguages = new List<Language>();
        foreach (var languageId in newLanguageIds)
        {
            var language = await _languageRepository.GetByIdAsync(languageId);
            if (language == null)
                throw new LanguageNotFoundException(languageId);
            validLanguages.Add(language);
        }

        // Получаем текущие языки
        var currentLanguageIds = task.TaskLanguages.Select(tl => tl.IdLanguage).ToList();
        
        // Находим изменения
        var languagesToAdd = newLanguageIds.Except(currentLanguageIds).ToList();
        var languagesToRemove = currentLanguageIds.Except(newLanguageIds).ToList();
        
        // Удаляем старые связи
        foreach (var languageId in languagesToRemove)
        {
            var taskLanguage = task.TaskLanguages.FirstOrDefault(tl => tl.IdLanguage == languageId);
            if (taskLanguage != null)
            {
                // Явно удаляем связь через репозиторий
                _repository.RemoveTaskLanguage(taskLanguage);
            }
        }
        
        // Добавляем новые связи
        foreach (var languageId in languagesToAdd)
        {
            var taskLanguage = new TaskLanguage(task.Id, languageId);
            // Явно добавляем связь через репозиторий
            await _repository.AddTaskLanguageAsync(taskLanguage);
        }
    }
    
   
}