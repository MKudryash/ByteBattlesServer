// ByteBattlesServer.Microservices.AuthService.Domain/Entities/RefreshToken.cs
namespace ByteBattlesServer.Microservices.AuthService.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime Expires { get; private set; }
    public DateTime Created { get; private set; }
    public string? CreatedByIp { get; private set; } // Nullable
    public DateTime? Revoked { get; private set; } // Nullable
    public string? RevokedByIp { get; private set; } // Nullable - ЭТО ВАЖНО
    public string? ReplacedByToken { get; private set; } // Nullable
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public User User { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expires, string createdByIp)
    {
        UserId = userId;
        Token = token;
        Expires = expires;
        Created = DateTime.UtcNow;
        CreatedByIp = createdByIp;
    }

    public void Revoke(string revokedByIp, string replacedByToken = null)
    {
        Revoked = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}