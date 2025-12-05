using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using ByteBattlesServer.Microservices.AuthService.Domain.Exceptions;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.AuthServices.Application.Commands;
using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicyService _passwordPolicyService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;
    private readonly IRoleRepository _roleRepository;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordPolicyService passwordPolicyService,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _passwordPolicyService = passwordPolicyService;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
        _roleRepository = roleRepository;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new ErrorRequest("User with this email already exists");

        var passwordValidationResult = _passwordPolicyService.ValidatePassword(request.Password);
        if (!passwordValidationResult.IsValid)
            throw new ErrorRequest($"Invalid password: {string.Join(", ", passwordValidationResult.Errors)}");

        // Determine which Role to assign
        Role userRole;
        if (!string.IsNullOrEmpty(request.Role.ToString()))
        {
            // Get Role by name
            userRole = await _roleRepository.GetByNameAsync(request.Role.ToString());
            if (userRole == null)
                throw new AuthException($"Role '{request.Role}' not found", "ROLE_NOT_FOUND");
        }
        else
        {
            // Use default Role
            userRole = await _roleRepository.GetByNameAsync("User");
            if (userRole == null)
                throw new AuthException("Default Role 'User' not found", "DEFAULT_ROLE_NOT_FOUND");
        }

        // Create user
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName, userRole);

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(7),
            "127.0.0.1"); // TODO: Get real IP from request

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Публикация события
        var userRegisteredEvent = new UserRegisteredIntegrationEvent
        (
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            true,
            request.Role
        );

        _messageBus.Publish(
            userRegisteredEvent,
            "user-events",
            "user.registered");

        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = new RoleDto()
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name,
                    Description =  user.Role.Description
                }
            }
        };

        return response;
    }
}