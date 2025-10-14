// ByteBattlesServer.Microservices.AuthService.Application/Handlers/RefreshTokenCommandHandler.cs

using System.Security.Claims;
using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using ByteBattlesServer.Microservices.AuthService.Domain.Exceptions;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.AuthServices.Application.Commands;
using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Получаем principal из старого access token
        ClaimsPrincipal principal;
        try
        {
            principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        }
        catch (SecurityTokenException)
        {
            throw new AuthException("Invalid access token", "INVALID_ACCESS_TOKEN");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new AuthException("Invalid token claims", "INVALID_TOKEN_CLAIMS");
        }

        // Ищем refresh token в базе
        var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        
        if (storedRefreshToken == null)
            throw new AuthException("Invalid refresh token", "INVALID_REFRESH_TOKEN");

        if (storedRefreshToken.UserId != userId)
            throw new AuthException("Refresh token does not match user", "TOKEN_MISMATCH");

        if (storedRefreshToken.IsExpired)
            throw new AuthException("Refresh token expired", "REFRESH_TOKEN_EXPIRED");

        if (storedRefreshToken.IsRevoked)
            throw new AuthException("Refresh token revoked", "REFRESH_TOKEN_REVOKED");

        // Отзываем старый refresh token
        storedRefreshToken.Revoke(request.IpAddress, "Replaced by refresh operation");
        _refreshTokenRepository.Update(storedRefreshToken);

        // Создаем новые токены
        var user = storedRefreshToken.User;
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken(
            user.Id,
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(7), // 7 дней для refresh token
            request.IpAddress);

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddHours(1), // 1 час для access token
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles?.Select(ur => ur.Role.Name) ?? Enumerable.Empty<string>()
            }
        };
    }
}