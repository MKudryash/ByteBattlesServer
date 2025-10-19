namespace ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Services;

public interface IPasswordPolicyService
{
    (bool IsValid, List<string> Errors) ValidatePassword(string password);
}
