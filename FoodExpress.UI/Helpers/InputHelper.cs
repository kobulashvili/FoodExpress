
namespace FoodExpress.UI.Helpers;

public static class InputHelper
{
    public static string ReadString(string message)
    {
        while (true)
        {
            Console.Write(message);

            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine("Value is required.");
        }
    }

    public static string ReadPassword(string message)
    {
        while (true)
        {
            Console.Write(message);

            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine("Password is required.");
        }
    }

    public static int ReadInt(string message)
    {
        while (true)
        {
            Console.Write(message);

            var input = Console.ReadLine();

            if (int.TryParse(input, out var value))
                return value;

            Console.WriteLine("Please enter a valid number.");
        }
    }

    public static decimal ReadDecimal(string message)
    {
        while (true)
        {
            Console.Write(message);

            var input = Console.ReadLine();

            if (decimal.TryParse(input, out var value))
                return value;

            Console.WriteLine("Please enter a valid number.");
        }
    }

    public static int ReadPositiveInt(string message)
    {
        while (true)
        {
            var value = ReadInt(message);

            if (value > 0)
                return value;

            Console.WriteLine(
                "Value must be greater than zero.");
        }
    }

    public static decimal ReadPositiveDecimal(string message)
    {
        while (true)
        {
            var value = ReadDecimal(message);

            if (value > 0)
                return value;

            Console.WriteLine(
                "Value must be greater than zero.");
        }
    }
}
