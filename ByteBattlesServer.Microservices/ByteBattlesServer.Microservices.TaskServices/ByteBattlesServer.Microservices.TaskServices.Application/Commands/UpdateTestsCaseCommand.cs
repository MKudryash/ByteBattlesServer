using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record UpdateTestsCaseCommand(
    Guid Id,
    string? Input,
    string? Output,
    bool IsExample):IRequest<TestCaseDto>;