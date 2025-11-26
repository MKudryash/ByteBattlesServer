using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Handlers;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ResponseCreateRoom>
{
    private readonly IBattleRoomRepository  _battleRoomRepository;
    private readonly IUnitOfWork  _unitOfWork;

    public CreateRoomCommandHandler(IBattleRoomRepository repository, IUnitOfWork unitOfWork)
    {
        _battleRoomRepository =  repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseCreateRoom> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = new BattleRoom(request.Name);
        
        room.AddParticipant(request.UserId);
        await _battleRoomRepository.AddAsync(room);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ResponseCreateRoom(){Id = room.Id};
    }
}

// public class SubmitCodeCommandHandler : IRequestHandler<SubmitCodeCommand, SubmitCodeResponse>
// {
//     private readonly IApplicationDbContext _context;
//     private readonly ICodeExecutionService _codeExecutionService;
//
//     public SubmitCodeCommandHandler(IApplicationDbContext context, ICodeExecutionService codeExecutionService)
//     {
//         _context = context;
//         _codeExecutionService = codeExecutionService;
//     }
//
//     public async Task<Result<Guid>> Handle(SubmitCodeCommand request, CancellationToken cancellationToken)
//     {
//         var room = await _context.BattleRooms
//             .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
//
//         if (room == null)
//             return Result<Guid>.Failure("Room not found");
//
//         if (!room.Participants.Any(p => p.UserId == request.UserId))
//             return Result<Guid>.Failure("User not in room");
//
//         var submission = new CodeSubmission(request.RoomId, request.UserId, request.ProblemId, request.Code);
//         
//         // Execute code asynchronously
//         var executionResult = await _codeExecutionService.ExecuteAsync(request.Code, request.ProblemId);
//         submission.SetResult(executionResult);
//
//         _context.CodeSubmissions.Add(submission);
//         await _context.SaveChangesAsync(cancellationToken);
//
//         return Result<Guid>.Success(submission.Id);
//     }
// }

// // Application/Queries/GetRoomQuery.cs
// public record GetRoomQuery(Guid RoomId) : IRequest<Result<RoomDto>>;
//
// public class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, Result<RoomDto>>
// {
//     private readonly IApplicationDbContext _context;
//
//     public GetRoomQueryHandler(IApplicationDbContext context)
//     {
//         _context = context;
//     }
//
//     public async Task<Result<RoomDto>> Handle(GetRoomQuery request, CancellationToken cancellationToken)
//     {
//         var room = await _context.BattleRooms
//             .Include(r => r.Participants)
//             .Include(r => r.Submissions)
//             .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
//
//         if (room == null)
//             return Result<RoomDto>.Failure("Room not found");
//
//         var dto = new RoomDto(
//             room.Id,
//             room.Name,
//             room.Status,
//             room.Participants.Select(p => p.UserId).ToList(),
//             room.Submissions.Select(s => new SubmissionDto(
//                 s.Id, s.UserId, s.ProblemId, s.SubmittedAt, s.Result
//             )).ToList()
//         );
//
//         return Result<RoomDto>.Success(dto);
//     }
// }
//
// // Application/DTOs
// public record RoomDto(Guid Id, string Name, RoomStatus Status, List<Guid> ParticipantIds, List<SubmissionDto> Submissions);
// public record SubmissionDto(Guid Id, Guid UserId, string ProblemId, DateTime SubmittedAt, SubmissionResult? Result);
//
// // Application/Interfaces/ICodeExecutionService.cs
// public interface ICodeExecutionService
// {
//     Task<SubmissionResult> ExecuteAsync(string code, string problemId);
// }