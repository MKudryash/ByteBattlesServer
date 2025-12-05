using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Domain.Enums;
using ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Handlers;

public class CloseRoomHandler : IRequestHandler<CloseRoom, CloseResponses>
{
    private readonly IBattleRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseRoomHandler(IBattleRoomRepository roomRepository, IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CloseResponses> Handle(CloseRoom request, CancellationToken cancellationToken)
    {
      var room =  await _roomRepository.GetByIdAsync(request.RoomId);

      if (room is null)
      {
        throw new BattleRoomNotFoundException(request.RoomId);
      }

      room.Status = RoomStatus.Completed;

      _roomRepository.UpdateAsync(room);
      _unitOfWork.SaveChangesAsync(cancellationToken);
      return new CloseResponses();
    }
}