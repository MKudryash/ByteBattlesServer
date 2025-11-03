using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
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

        var language = new Language(request.LanguageTitle, request.LanguageShortTitle);

        await _languageRepository.AddAsync(language);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return MapToDto(language);

    }

    private LanguageDto MapToDto(Language language) => new()
    {
        Id = language.Id,
        Title = language.Title,
        ShortTitle = language.ShortTitle,
        CompilerCommand = language.CompilerCommand,
        ExecutionCommand = language.ExecutionCommand,
        FileExtension = language.FileExtension,
        SupportsCompilation = language.SupportsCompilation,
    };
}