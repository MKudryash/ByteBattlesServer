using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;


    public class GetLanguageByIdQueryHandler: IRequestHandler<GetLanguageByIdQuery, LanguageDto?>
    {
        private readonly ILanguageRepository _repository;
        public GetLanguageByIdQueryHandler(ILanguageRepository repository) => _repository = repository;
        

        public async Task<LanguageDto?> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
        {
            var language = await _repository.GetByIdAsync(request.LanguageId);
            if (language == null)
                throw new LanguageNotFoundException(request.LanguageId);
            return MapToDto(language);
        }
        private LanguageDto MapToDto(Language language) => new()
        {
            Id = language.Id,
            Title = language.Title,
            ShortTitle = language.ShortTitle
        };
    }