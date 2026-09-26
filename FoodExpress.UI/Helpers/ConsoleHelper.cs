namespace FoodExpress.UI.Helpers;

public static class ConsoleHelper
{
    public static void Clear()
    {
        Console.Clear();
    }

    public static void ShowTitle(string title)
    {
        Console.Clear();

        Console.WriteLine("========================================");
        Console.WriteLine($"          {title}");
        Console.WriteLine("========================================");
        Console.WriteLine();
    }

    public static void Pause()
    {
        Console.ReadKey();
    }

    public static void ShowError(string message)
    {
        Console.WriteLine();
        Console.WriteLine($"Error: {message}");
    }

    public static void ShowSuccess(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
    }
}
