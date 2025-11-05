using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class RemoveLanguageCommandHandler : IRequestHandler<RemoveLanguageCommand, DeleteResponseDto>
{
    private readonly ILanguageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveLanguageCommandHandler(ILanguageRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }


    public async Task<DeleteResponseDto> Handle(RemoveLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _repository.GetByIdAsync(request.LanguageId);
        if (language == null)
            throw new LanguageNotFoundException(request.LanguageId);


        var taskLanguages = await _repository.GetTaskLanguagesAsync(language.Id);

        if (taskLanguages.Any())
        {
            throw new LanguageInUseException(request.LanguageId, taskLanguages.Count);
        }


        _repository.Delete(language);
        _unitOfWork.SaveChangesAsync();

        return new DeleteResponseDto("Task language successfully");
    }
}