using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class UpdateLibraryCommandHandler : IRequestHandler<UpdateLibraryCommand, LibraryDto>
{
    private readonly ILanguageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLibraryCommandHandler(ILanguageRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task<LibraryDto> Handle(UpdateLibraryCommand request, CancellationToken cancellationToken)
    {
        var library = await _repository.GetLibraryByIdAsync(request.Id);
        if (library == null)
            throw new LibraryNotFoundException(request.Id);
        
        library.Update(request.Name,request.Description,request.Version);
        
        _repository.UpdateLibraryAsync(library);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return LibraryMappings.MapToDto(library);
    }
}