using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Handlers;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public class GetLibraryQueryHandler : IRequestHandler<GetLibraryQuery, List<LibraryDto>>
{
    private readonly ILanguageRepository _repository;
    public GetLibraryQueryHandler(ILanguageRepository repository) => _repository = repository;

    public async Task<List<LibraryDto>> Handle(GetLibraryQuery request, CancellationToken cancellationToken)
    {
        var language = await _repository.GetByIdAsync(request.languageId);
        if(language == null)
            throw new LanguageNotFoundException(request.languageId);
        
        var libraries = await _repository.GetLibrariesByLanguageIdAsync(request.languageId);
        if(libraries == null)
            throw new LibraryNotFoundException(request.languageId);
        return libraries.Select(l=>LibraryMappings.MapToDto(l)).ToList();
    }
}