using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;

public class LanguageAlreadyExistsException:LanguageException
{
    public LanguageAlreadyExistsException(Guid languageId) 
        : base($"Language already exists for language ID: {languageId}", "LANGUAGE_ALREADY_EXISTS")
    {
    } 
    public LanguageAlreadyExistsException(string title) 
        : base($"Language already exists for language title: {title}", "LANGUAGE_ALREADY_EXISTS")
    {
    }
}