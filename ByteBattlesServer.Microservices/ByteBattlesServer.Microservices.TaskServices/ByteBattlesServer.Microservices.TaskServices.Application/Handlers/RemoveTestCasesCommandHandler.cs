using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class RemoveTestCasesCommandHandler:IRequestHandler<RemoveTestCasesCommand, DeleteResponseDto>
{
    
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTestCasesCommandHandler(ITaskRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task<DeleteResponseDto> Handle(RemoveTestCasesCommand request, CancellationToken cancellationToken)
    {
        var testCase = await _repository.GetTestCaseByIdAsync(request.Id);
        if (testCase == null)
            throw new TestCaseNotFoundException(request.Id);
        
        _repository.RemoveTestCaseAsync(testCase);
        _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new DeleteResponseDto();
    }
}