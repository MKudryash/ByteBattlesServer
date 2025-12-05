using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, LanguageDto>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLanguageCommandHandler(ILanguageRepository languageRepository, IUnitOfWork unitOfWork)
    {
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LanguageDto> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _languageRepository.GetByIdAsync(request.LanguageId);
        if (language == null)
            throw new LanguageNotFoundException(request.LanguageId);
        
        language.Update(request.LanguageTitle, request.LanguageShortTitle, request.FileExtension,request.CompilerCommand,
            request.SupportsCompilation,  request.ExecutionCommand, request.PatternFunction, request.PatternMain);
        
        _languageRepository.Update(language);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return LanguageMappings.MapToDto(language);
    }
}