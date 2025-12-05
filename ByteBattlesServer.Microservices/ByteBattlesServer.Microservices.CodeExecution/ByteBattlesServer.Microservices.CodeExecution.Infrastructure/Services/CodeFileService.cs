using System.Text;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class CodeFileService
{
    private readonly ILogger<CodeFileService> _logger;

    public CodeFileService(ILogger<CodeFileService> logger)
    {
        _logger = logger;
    }

    public async Task<string> CreateCodeFileAsync(string code, string fileExtension)
    {
        var tempDir = Path.GetTempPath();
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(tempDir, fileName);

        try
        {
            // Очищаем код от escape-последовательностей
            var cleanedCode = CleanCodeFromEscapes(code);
            
            await File.WriteAllTextAsync(filePath, cleanedCode, Encoding.UTF8);

            _logger.LogInformation("🟢 [CodeFileService] Created code file: {FilePath}", filePath);
            _logger.LogDebug("🟢 [CodeFileService] Code preview (first 200 chars): {CodePreview}", 
                cleanedCode.Length > 200 ? cleanedCode.Substring(0, 200) + "..." : cleanedCode);
            
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [CodeFileService] Failed to create code file: {FilePath}", filePath);
            throw;
        }
    }

    public string CleanCodeFromEscapes(string code)
    {
        if (string.IsNullOrEmpty(code))
            return code;

        // Заменяем escape-последовательности на реальные символы
        var cleaned = code
            .Replace("\\n", "\n")
            .Replace("\\t", "\t")
            .Replace("\\r", "\r")
            .Replace("\\\"", "\"")
            .Replace("\\'", "'")
            .Replace("\\\\", "\\");

        _logger.LogDebug("🟠 [CodeFileService] Cleaned code from escapes. Original length: {OriginalLength}, Cleaned length: {CleanedLength}", 
            code.Length, cleaned.Length);

        return cleaned;
    }

    public async Task CleanupCodeFileAsync(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("🟢 [CodeFileService] Cleaned up code file: {FilePath}", filePath);
            }

            // Также удаляем скомпилированный файл если он существует
            var compiledPath = Path.ChangeExtension(filePath, "");
            if (File.Exists(compiledPath))
            {
                File.Delete(compiledPath);
                _logger.LogInformation("🟢 [CodeFileService] Cleaned up compiled file: {FilePath}", compiledPath);
            }

            // Для C# удаляем .dll файлы
            var dllPath = Path.ChangeExtension(filePath, ".dll");
            if (File.Exists(dllPath))
            {
                File.Delete(dllPath);
                _logger.LogInformation("🟢 [CodeFileService] Cleaned up DLL file: {FilePath}", dllPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("🟡 [CodeFileService] Failed to cleanup code file: {FilePath}, Error: {Error}", 
                filePath, ex.Message);
        }
    }
}