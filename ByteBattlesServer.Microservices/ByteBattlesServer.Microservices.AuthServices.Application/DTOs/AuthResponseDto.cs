namespace ByteBattlesServer.Microservices.AuthServices.Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; }
}