using ByteBattlesServer.Domain.Results;

namespace ByteBattlesServer.Microservices.AuthService.Domain.ValueObject;



public class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ErrorRequest("Email cannot be empty");

        if (!IsValidEmail(email))
            throw new ErrorRequest("Email is invalid");

        return new Email(email.ToLower());
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}