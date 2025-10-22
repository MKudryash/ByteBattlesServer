using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record AddTestCasesCommand(
    Guid TaskId,
    List<CreateTestCaseDto> TestCases
    ):IRequest<List<TestCaseDto>>;