using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class CreateLibrariesCommandHandler: IRequestHandler<CreateLibrariesCommand, List<LibraryDto>>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLibrariesCommandHandler( ILanguageRepository languageRepository, IUnitOfWork unitOfWork)
    {
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<List<LibraryDto>> Handle(CreateLibrariesCommand request, CancellationToken cancellationToken)
    {
        var language = await _languageRepository.GetByIdAsync(request.LanguageId);
        if (language is null)
            throw new LanguageNotFoundException(request.LanguageId);
        var createdLibraries =  new List<Library>();

        foreach (var libraryDto in request.Libraries)
        {
            var library = new Library(
                libraryDto.NameLibrary,
                libraryDto.Description,
                libraryDto.Version,
                request.LanguageId);
            await _languageRepository.AddLibraryAsync(library);
            createdLibraries.Add(library);
        }
        
        language.UpdatedAd = DateTime.UtcNow;
        await _languageRepository.Update(language);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return createdLibraries.Select(cL=>LibraryMappings.MapToDto(cL)).ToList();
    }
}