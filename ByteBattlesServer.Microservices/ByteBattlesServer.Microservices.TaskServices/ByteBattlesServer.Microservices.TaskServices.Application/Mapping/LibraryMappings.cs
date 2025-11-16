using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Mapping;

public static class LibraryMappings
{
    public static LibraryDto MapToDto(Library library)
    {
        return new LibraryDto
        {
            Id = library.Id,
            Name = library.NameLibrary,
            Description = library.Description,
            Version = library.Version,
            languageId = library.LanguageId
        };
    }
}