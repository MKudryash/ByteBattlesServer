using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, LanguageDto>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLanguageCommandHandler( ILanguageRepository languageRepository, IUnitOfWork unitOfWork)
    {
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<LanguageDto> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var existingLanguage = await _languageRepository.GetByNameAsync(request.LanguageTitle);
        if (existingLanguage!= null)
            throw new LanguageNotFoundException(request.LanguageTitle);

        var language = new Language(request.LanguageTitle, request.LanguageShortTitle,
            request.FileExtension, request.CompilerCommand, request.ExecutionCommand, request.SupportsCompilation, request.PatternFunction,
            request.PatternMain);

        await _languageRepository.AddAsync(language);
        
        
        var createdLibraries =  new List<Library>();

        foreach (var libraryDto in request.Libraries)
        {
            var library = new Library(
                libraryDto.NameLibrary,
                libraryDto.Description,
                libraryDto.Version,
                language.Id);
            await _languageRepository.AddLibraryAsync(library);
            createdLibraries.Add(library);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return LanguageMappings.MapToDto(language);

    }
}