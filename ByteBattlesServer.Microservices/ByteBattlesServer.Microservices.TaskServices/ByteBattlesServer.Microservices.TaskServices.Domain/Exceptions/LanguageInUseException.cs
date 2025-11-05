using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;

public class LanguageInUseException:LanguageException
{
    public LanguageInUseException(Guid languageId, int usageCount)
        : base($"Language with ID '{languageId}' is used in {usageCount} tasks and cannot be deleted")
    {
       
    }
}