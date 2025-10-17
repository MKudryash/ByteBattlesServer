namespace ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

public enum UserLevel
{
    Beginner = 1,      // 0-999 XP
    Novice = 2,        // 1000-2999 XP
    Intermediate = 3,  // 3000-5999 XP
    Advanced = 4,      // 6000-9999 XP
    Expert = 5,        // 10000-14999 XP
    Master = 6,        // 15000-24999 XP
    GrandMaster = 7    // 25000+ XP
}