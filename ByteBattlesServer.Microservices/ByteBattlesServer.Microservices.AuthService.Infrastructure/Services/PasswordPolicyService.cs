using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Services;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Services;



public class PasswordPolicyService : IPasswordPolicyService
{
    public (bool IsValid, List<string> Errors) ValidatePassword(string password)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(password))
            errors.Add("Password cannot be empty");
            
        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long");
            
        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");
            
        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");
            
        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit");
        
        if (IsCommonPassword(password))
            errors.Add("Password is too common");

        return (!errors.Any(), errors);
    }
    
    private bool IsCommonPassword(string password)
    {
        var commonPasswords = new[] { "password", "123456", "qwerty" };
        return commonPasswords.Contains(password.ToLower());
    }
}