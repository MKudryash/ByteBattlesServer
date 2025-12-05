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
        // Получаем задачу с включенными языками и тестовыми случаями
        var task = await _repository.GetByIdAsyncWithTasks(request.TaskId);
        
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
        
        // Обновляем библиотеки если переданы
        if (request.LibrariesIds != null && request.LibrariesIds.Any())
        {
            await UpdateTaskLibrariesAsync(task, request.LibrariesIds);
        }
        
        // Обновляем тестовые случаи если переданы
        if (request.TestCases != null && request.TestCases.Any())
        {
            await UpdateTaskTestCasesAsync(task, request.TestCases);
        }
        
        // Явно отмечаем задачу как измененную
        _repository.Update(task);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Перезагружаем задачу с актуальными данными
        var updatedTask = await _repository.GetByIdAsyncWithTasks(request.TaskId);
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
    
    private async Task UpdateTaskLibrariesAsync(Domain.Entities.Task task, List<Guid> newLibrariesIds)
    {
        // Валидируем новые библиотеки
        var validLibraries = new List<Library>();
        foreach (var libraryId in newLibrariesIds)
        {
            var library = await _languageRepository.GetLibraryByIdAsync(libraryId);
            if (library == null)
                throw new LibraryNotFoundException(libraryId);
            validLibraries.Add(library);
        }

        // Получаем текущие библиотеки
        var currentLibrariesIds = task.Libraries.Select(tl => tl.IdLibrary).ToList();
        
        // Находим изменения
        var librariesToAdd = newLibrariesIds.Except(currentLibrariesIds).ToList();
        var librariesToRemove = currentLibrariesIds.Except(newLibrariesIds).ToList();
        
        // Удаляем старые связи
        foreach (var libraryId in librariesToRemove)
        {
            var library = task.Libraries.FirstOrDefault(tl => tl.IdLibrary == libraryId);
            if (library != null)
            {
                _repository.RemoveTaskLibrary(library);
            }
        }
        
        // Добавляем новые связи
        foreach (var libraryId in librariesToAdd)
        {
            var library = new TaskLibrary(task.Id, libraryId);
            // Явно добавляем связь через репозиторий
            await _repository.AddTaskLibraryAsync(library);
        }
    }
    
    private async Task UpdateTaskTestCasesAsync(Domain.Entities.Task task, List<TestCaseDto> newTestCases)
    {
        // Получаем текущие тестовые случаи
        var currentTestCases = task.TestCases?.ToList() ?? new List<TestCases>();
        
        // Создаем словарь для быстрого поиска по входным данным
        var currentTestCasesDict = currentTestCases.ToDictionary(tc => tc.Input);
        
        // Списки для операций
        var testCasesToAdd = new List<TestCases>();
        var testCasesToUpdate = new List<TestCases>();
        var testCasesToRemove = new List<TestCases>();
        
        // Обрабатываем новые тестовые случаи
        foreach (var testCaseDto in newTestCases)
        {
            if (currentTestCasesDict.TryGetValue(testCaseDto.Input, out var existingTestCase))
            {
                // Обновляем существующий тестовый случай
                existingTestCase.Update(
                    testCaseDto.Input,
                    testCaseDto.Output,
                    testCaseDto.IsExample);
                testCasesToUpdate.Add(existingTestCase);
            }
            else
            {
                // Создаем новый тестовый случай
                var newTestCase = new TestCases(
                    task.Id,
                    testCaseDto.Input,
                    testCaseDto.Output,
                    testCaseDto.IsExample);
                testCasesToAdd.Add(newTestCase);
            }
        }
        
        // Находим тестовые случаи для удаления (которые отсутствуют в новых данных)
        var newInputs = newTestCases.Select(tc => tc.Input).ToHashSet();
        foreach (var existingTestCase in currentTestCases)
        {
            if (!newInputs.Contains(existingTestCase.Input))
            {
                testCasesToRemove.Add(existingTestCase);
            }
        }
        
        // Выполняем операции с тестовыми случаями
        foreach (var testCase in testCasesToRemove)
        {
            _repository.RemoveTestCaseAsync(testCase);
        }
        
        foreach (var testCase in testCasesToAdd)
        {
            await _repository.AddTestCaseAsync(testCase);
        }
        
        foreach (var testCase in testCasesToUpdate)
        {
            _repository.UpdateTestCaseAsync(testCase);
        }
    }
}