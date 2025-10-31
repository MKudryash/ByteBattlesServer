using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class FileService : IFileService
{
    
    public async Task WriteToFileAsync(string content, string filePath)
    {
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        await Task.CompletedTask;
    }

    public string GetTempFilePath(string extension)
    {
        var tempPath = Path.GetTempPath();
        var fileName = $"{Guid.NewGuid()}{extension}";
        
        return Path.Combine(tempPath, fileName);
    }
}