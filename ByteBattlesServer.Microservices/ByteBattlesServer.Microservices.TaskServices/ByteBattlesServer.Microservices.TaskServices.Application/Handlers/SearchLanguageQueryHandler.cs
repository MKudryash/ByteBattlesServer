using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class SearchLanguageQueryHandler: IRequestHandler<SearchLanguagesQuery, List<LanguageDto>>
{
    private readonly ILanguageRepository _repository;
    
    public SearchLanguageQueryHandler(ILanguageRepository repository)=> _repository = repository;
    
    public async Task<List<LanguageDto>> Handle(SearchLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languages = await _repository.SearchLanguages(request.SearchTerm);
        
        return languages.Select(x=>LanguageMappings.MapToDto(x)).ToList();
    }
}