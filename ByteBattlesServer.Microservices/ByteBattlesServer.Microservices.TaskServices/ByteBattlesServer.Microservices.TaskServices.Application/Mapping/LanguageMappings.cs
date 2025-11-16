using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Mapping;

public static class LanguageMappings
{
    public static LanguageDto MapToDto(Language language)
    {
        if (language == null) return null;
        
        return new LanguageDto
        {
            Id = language.Id,
            Title = language.Title,
            ShortTitle = language.ShortTitle,
            CompilerCommand = language.CompilerCommand,
            ExecutionCommand = language.ExecutionCommand,
            FileExtension = language.FileExtension,
            SupportsCompilation = language.SupportsCompilation,
            Pattern = language.Pattern,
            Libraries = language.Libraries.Select(x=> new LibraryDto()
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.NameLibrary,
                Version = x.Version,
                languageId = language.Id,
            }).ToList(),
        };
    }
    
    public static Language MapToEntity(CreateLanguageCommand command)
    {
        return new Language(
            command.LanguageTitle,
            command.LanguageShortTitle,
            command.FileExtension,
            command.CompilerCommand,
            command.ExecutionCommand,
            command.SupportsCompilation,
            command.Pattern
        );
    }
}