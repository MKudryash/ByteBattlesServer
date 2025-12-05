using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattlesServer.Domain.Results;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Handlers;

public class JoinRoomCommandHandler : IRequestHandler<JoinRoomCommand, ResponseSuccessJoin>
{
    private readonly IBattleRoomRepository  _battleRoomRepository;
    private readonly IUnitOfWork  _unitOfWork;

    public JoinRoomCommandHandler(IBattleRoomRepository repository, IUnitOfWork unitOfWork)
    {
        _battleRoomRepository =  repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseSuccessJoin> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _battleRoomRepository.GetByIdAsync(request.RoomId);

        if (room == null)
            throw new BattleRoomNotFoundException(request.RoomId);

        try
        {
            room.AddParticipant(request.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new ResponseSuccessJoin();
        }
        catch (InvalidOperationException ex)
        {
            throw new ErrorRequest(ex.Message);
        }
    }
}