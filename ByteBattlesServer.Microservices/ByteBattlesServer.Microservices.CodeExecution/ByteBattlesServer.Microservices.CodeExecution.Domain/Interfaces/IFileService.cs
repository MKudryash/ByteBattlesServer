namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

public interface IFileService
{
    Task WriteToFileAsync(string content, string filePath);
    Task DeleteFileAsync(string filePath);
    string GetTempFilePath(string extension);
}