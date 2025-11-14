using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using ByteBattlesServer.Microservices.AuthService.Domain.Exceptions;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.AuthServices.Application.Commands;
using ByteBattlesServer.Microservices.AuthServices.Application.DTOs;
using MediatR;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;

namespace ByteBattlesServer.Microservices.AuthServices.Application.Handlers;


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
        IRoleRepository  roleRepository,
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
        var passwordValidationResult =  _passwordPolicyService.ValidatePassword(request.Password);
        if (!passwordValidationResult.IsValid)
            throw new ErrorRequest($"Invalid password: {string.Join(", ", passwordValidationResult.Errors)}");

        // Create user
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);



        var rolesToAdd = new List<Role>();

        if (request.Roles != null && request.Roles.Any())
        {
            // Получаем существующие роли из базы
            var existingRoles = await _roleRepository.GetRolesByNamesAsync(request.Roles);
            var existingRoleNames = existingRoles.Select(r => r.Name).ToHashSet();

            // Проверяем, все ли запрошенные роли существуют
            var missingRoles = request.Roles.Except(existingRoleNames).ToList();
            if (missingRoles.Any())
            {
                throw new AuthException($"Roles not found: {string.Join(", ", missingRoles)}", "ROLES_NOT_FOUND");
            }

            rolesToAdd.AddRange(existingRoles);
        }
        else
        {
            // Добавляем роль по умолчанию
            var defaultRole = await _roleRepository.GetByNameAsync("user");
            if (defaultRole == null)
            {
                throw new AuthException("Default role 'user' not found", "DEFAULT_ROLE_NOT_FOUND");
            }
            rolesToAdd.Add(defaultRole);
        }

        // Добавляем роли пользователю
        foreach (var role in rolesToAdd)
        {
            user.AddRole(role);
        }

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        var refreshTokenEntity = new RefreshToken(
            user.Id, 
            refreshToken, 
            DateTime.UtcNow.AddDays(7), 
            "127.0.0.1");

        // Публикация события
        var userRegisteredEvent = new UserRegisteredIntegrationEvent
        {
            UserId = user.Id,
            Email = user.Email.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RegisteredAt = DateTime.UtcNow
        };

        _messageBus.Publish(
            userRegisteredEvent,
            "user-events",
            "user.registered");
        
        
        
        
        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
                Roles = user.UserRoles.Select(r=>r.Role.Name).ToList()
            }
        };

        return response;
    }
}