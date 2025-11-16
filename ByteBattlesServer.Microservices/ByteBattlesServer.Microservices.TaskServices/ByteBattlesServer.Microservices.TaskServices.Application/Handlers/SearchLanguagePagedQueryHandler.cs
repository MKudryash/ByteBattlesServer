using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class SearchLanguagePagedQueryHandler: IRequestHandler<SearchLanguagesPagedQuery, List<LanguageDto>>
{
    private readonly ILanguageRepository _repository;
    
    public SearchLanguagePagedQueryHandler(ILanguageRepository repository)=> _repository = repository;
    
    public async Task<List<LanguageDto>> Handle(SearchLanguagesPagedQuery request, CancellationToken cancellationToken)
    {
        var languages = await _repository.SearchLanguagesPagedAsync(request.SearchTerm
            ,request.Page, request.PageSize);
        
        return languages.Select(x=>LanguageMappings.MapToDto(x)).ToList();
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
        Pattern = language.Pattern,
    };
}