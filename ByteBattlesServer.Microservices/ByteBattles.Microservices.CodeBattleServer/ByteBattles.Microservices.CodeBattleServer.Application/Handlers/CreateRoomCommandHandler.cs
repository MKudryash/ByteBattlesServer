using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattles.Microservices.CodeBattleServer.Domain.Models;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Handlers;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ResponseCreateRoom>
{
    private readonly IBattleRoomRepository _battleRoomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaskLanguageService _taskLanguageService;

    public CreateRoomCommandHandler(IBattleRoomRepository repository, IUnitOfWork unitOfWork,
        ITaskLanguageService taskLanguageService)
    {
        _battleRoomRepository = repository;
        _unitOfWork = unitOfWork;
        _taskLanguageService = taskLanguageService;
    }

    public async Task<ResponseCreateRoom> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {

        var task = await _taskLanguageService.GetTaskInfoAsync(request.LanguageId, request.Difficulty);

        if (task == null)
        {
            throw new ArgumentException($"Language with ID {request.LanguageId} not found");
        }

        var room = new BattleRoom(request.Name, request.LanguageId, request.Difficulty);

        room.TaskId = task.Id;

        room.AddParticipant(request.UserId);
        await _battleRoomRepository.AddAsync(room);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Console.WriteLine("Task:" + task.TestCases.Count);
        
        Console.WriteLine("Task Language" + task.Language.Title);
        return new ResponseCreateRoom()
        {
            Id = room.Id, 
            TaskInfo = task
        };
    }
}