namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class CreateTestCaseDto
{
    public string Input { get; set; }
    public string Output { get; set; }
}
public class CreateTestCasesDto
{
   public List<CreateTestCaseDto> TestCases { get; set; }
}