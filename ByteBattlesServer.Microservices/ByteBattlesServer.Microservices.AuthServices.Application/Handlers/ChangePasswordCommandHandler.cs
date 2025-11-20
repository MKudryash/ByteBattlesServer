using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.AuthServices.Application.Commands;
using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using MediatR;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Handlers;

public class ChangePasswordCommandHandler:IRequestHandler<ChangePasswordCommand, ChangePasswordResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }
    public async Task<ChangePasswordResponseDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.userId);
        if (user is null)
            throw new UserProfileNotFoundException(request.userId);
        
        if (!_passwordHasher.VerifyPassword(request.OldPassword, user.PasswordHash))
            throw new ErrorRequest("Invalid password");
        
        user.ChangePassword(request.NewPassword);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ChangePasswordResponseDto();
    }
}