using ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using MediatR;

namespace ByteBattlesServer.Microservices.CodeExecution.Application.Commands;

public record TestCodeCommand(string Code, Guid Language, List<TestCaseDto> TestCases )
    :IRequest<CodeTestResultResponse>;