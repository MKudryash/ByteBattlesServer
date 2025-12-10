using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using MediatR;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateUserProfileCommandHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        
        if (userProfile == null)
        {
            throw new UserProfileNotFoundException(request.UserId);
        }

        userProfile.UpdateProfile(request.UserName, request.Bio,
            request.Country, request.GitHubUrl,
            request.LinkedInUrl, request.IsPublic);
        userProfile.UpdatedAt = DateTime.UtcNow;
        
        _userProfileRepository.Update(userProfile);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);


        return new UserProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            UserName = userProfile.UserName,
            Bio = userProfile.Bio,
            Country = userProfile.Country,
            GitHubUrl = userProfile.GitHubUrl,
            LinkedInUrl = userProfile.LinkedInUrl,
            IsPublic = true,
        };
    }
}