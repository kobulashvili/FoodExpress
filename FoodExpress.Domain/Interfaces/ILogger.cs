namespace FoodExpress.Domain.Interfaces;

public interface ILogger
{
    Task LogInfoAsync(string message);
    Task LogErrorAsync(string message, Exception? exception = null);
}