using ByteBattlesServer.Microservices.AuthService.Domain.Exceptions;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthServices.Application.Commands;
using MediatR;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Handlers;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        
        if (refreshToken == null)
            throw new AuthException("Refresh token not found", "TOKEN_NOT_FOUND");

        if (refreshToken.IsRevoked)
            throw new AuthException("Refresh token already revoked", "TOKEN_ALREADY_REVOKED");

        refreshToken.Revoke(request.IpAddress, "Revoked by user");
        _refreshTokenRepository.Update(refreshToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}