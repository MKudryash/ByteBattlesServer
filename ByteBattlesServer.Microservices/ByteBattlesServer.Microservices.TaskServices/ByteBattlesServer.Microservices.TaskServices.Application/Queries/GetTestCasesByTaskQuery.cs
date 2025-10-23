using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record GetTestCasesByTaskQuery(Guid TaskId) : IRequest<List<TestCaseDto>>;