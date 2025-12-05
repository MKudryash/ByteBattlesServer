using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class RemoveLibraryCommandHandler : IRequestHandler<RemoveLibrariesCommand, DeleteResponseDto>
{
    private readonly ILanguageRepository _repository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveLibraryCommandHandler(ILanguageRepository repository, IUnitOfWork unitOfWork,
        ITaskRepository taskRepository)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _taskRepository = taskRepository;
    }
    public async Task<DeleteResponseDto> Handle(RemoveLibrariesCommand request, CancellationToken cancellationToken)
    {
        var library = await _repository.GetLibraryByIdAsync(request.Id);
        if (library == null)
            throw new TestCaseNotFoundException(request.Id);
        
        var taskLibraries = await _taskRepository.GetTaskLibraryAsync(library.Id);
        
        if (taskLibraries.Any())
        {
            throw new LibraryInUseException(request.Id,taskLibraries.Count);
        }

        
        
        _repository.DeleteLibraryAsync(library);
        _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new DeleteResponseDto();
    }
}