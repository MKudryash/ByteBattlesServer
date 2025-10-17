namespace ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

public class UserProfileAlreadyExistsException : UserProfileException
{
    public UserProfileAlreadyExistsException(Guid userId) 
        : base($"User profile already exists for user ID: {userId}", "USER_PROFILE_ALREADY_EXISTS")
    {
    }
}