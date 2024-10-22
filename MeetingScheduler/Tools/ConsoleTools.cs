namespace MeetingScheduler.Tools;


internal static class ConsoleTools
{
    public static string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    public static DateTime GetDateTimeInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out var dateTime))
                return dateTime;

            Console.WriteLine("Неверный формат даты/времени. Попробуйте снова.");
        }
    }

    public static TimeSpan? GetOptionalTimeSpanInput(string prompt)
    {
        var input = GetInput(prompt);
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (TimeSpan.TryParse(input, out var timeSpan))
            return timeSpan;

        Console.WriteLine("Неверный формат времени. Время не установлено.");
        return null;
    }

    public static void WriteError(string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = color;
    }
}
