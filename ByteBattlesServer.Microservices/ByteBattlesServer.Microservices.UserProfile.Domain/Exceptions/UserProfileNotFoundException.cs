namespace ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

public class UserProfileNotFoundException : UserProfileException
{
    public UserProfileNotFoundException(Guid userId) 
        : base($"User profile not found for user ID: {userId}", "USER_PROFILE_NOT_FOUND")
    {
    }

    public UserProfileNotFoundException(string userName) 
        : base($"User profile not found for username: {userName}", "USER_PROFILE_NOT_FOUND")
    {
    }
}