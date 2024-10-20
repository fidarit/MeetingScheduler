namespace MeetingScheduler;


internal class Program
{
    static void Main(string[] args)
    {
        var manager = new MeetingManager();
        bool running = true;

        while (running)
        {
            Console.WriteLine("""
                Выберите действие:
                1. Добавить встречу
                2. Удалить встречу
                3. Обновить встречу
                4. Посмотреть встречи
                5. Экспортировать встречи в файл
                6. Выйти
                """);

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddMeeting(manager);
                    break;
                case "2":
                    RemoveMeeting(manager);
                    break;
                case "3":
                    UpdateMeeting(manager);
                    break;
                case "4":
                    ViewMeetings(manager);
                    break;
                case "5":
                    ExportMeetings(manager);
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                    break;
            }

            Console.WriteLine();
        }
    }

    static void AddMeeting(MeetingManager manager)
    {
        throw new NotImplementedException();
    }

    static void RemoveMeeting(MeetingManager manager)
    {
        throw new NotImplementedException();
    }

    static void UpdateMeeting(MeetingManager manager)
    {
        throw new NotImplementedException();
    }

    static void ViewMeetings(MeetingManager manager)
    {
        throw new NotImplementedException();
    }

    static void ExportMeetings(MeetingManager manager)
    {
        throw new NotImplementedException();
    }
}
