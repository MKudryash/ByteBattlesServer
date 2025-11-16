using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class RemoveLibraryCommandHandler : IRequestHandler<RemoveLibrariesCommand, DeleteResponseDto>
{
    private readonly ILanguageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveLibraryCommandHandler(ILanguageRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task<DeleteResponseDto> Handle(RemoveLibrariesCommand request, CancellationToken cancellationToken)
    {
        var testCase = await _repository.GetLibraryByIdAsync(request.Id);
        if (testCase == null)
            throw new TestCaseNotFoundException(request.Id);
        
        _repository.DeleteLibraryAsync(testCase);
        _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new DeleteResponseDto();
    }
}