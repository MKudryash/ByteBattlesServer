namespace ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

public class ProfileAccessDeniedException : UserProfileException
{
    public ProfileAccessDeniedException(Guid profileId) 
        : base($"Access denied to profile: {profileId}", "PROFILE_ACCESS_DENIED")
    {
    }
}