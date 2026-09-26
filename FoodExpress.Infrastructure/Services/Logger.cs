using FoodExpress.Domain.Interfaces;

namespace FoodExpress.Infrastructure.Services;

public class Logger : ILogger
{
    private readonly FileManager _fileManager;

    private const string FileName = "Logs.txt";

    public Logger(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public async Task LogInfoAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        var log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [INFO] {message}";

        await _fileManager.AppendLineAsync(
            FileName,
            log);
    }

    public async Task LogErrorAsync(
        string message,
        Exception? exception = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = "Unknown error.";

        var log =
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] {message}";

        if (exception != null)
        {
            log += $" | Exception: {exception.Message}";
        }

        await _fileManager.AppendLineAsync(
            FileName,
            log);
    }
}