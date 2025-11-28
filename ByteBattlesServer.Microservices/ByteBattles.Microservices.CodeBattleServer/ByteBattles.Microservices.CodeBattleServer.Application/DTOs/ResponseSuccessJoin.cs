namespace ByteBattles.Microservices.CodeBattleServer.Application.DTOs;

public record ResponseSuccessJoin(string Message = "Join successfully");
public record ResponseSuccessLeave(string Message = "Leave successfully");