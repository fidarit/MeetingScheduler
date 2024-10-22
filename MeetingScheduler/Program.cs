using MeetingScheduler.Models;
using MeetingScheduler.Services;
using static MeetingScheduler.Tools.ConsoleTools;


namespace MeetingScheduler;


internal class Program
{
    static void Main(string[] args)
    {
        var manager = new MeetingManager();
        using var notificationSystem = new NotificationSystem(manager);
        notificationSystem.OnNotification += (_, e) => Console.WriteLine($"Напоминание: Встреча '{e.Meeting.Title}' начнется в {e.Meeting.StartTime}");

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
        var title = GetInput("Введите название встречи: ");
        var startTime = GetDateTimeInput("Введите время начала (гггг-мм-дд чч:мм): ");
        var plannedEndTime = GetDateTimeInput("Введите время окончания (гггг-мм-дд чч:мм): ");
        var reminderTime = GetOptionalTimeSpanInput("Введите время напоминания (чч:мм) или оставьте пустым: ");

        try
        {
            var meeting = new Meeting(title, startTime, plannedEndTime, reminderTime);
            manager.AddMeeting(meeting);
            Console.WriteLine("Встреча успешно добавлена.");
        }
        catch (Exception e)
        {
            WriteError(e.Message);
        }
    }

    static void RemoveMeeting(MeetingManager manager)
    {
        var title = GetInput("Введите название встречи для удаления: ");
        var meetingToRemove = manager.GetMeetings(DateTime.Now).Find(m => m.Title == title);

        if (meetingToRemove != null)
        {
            manager.RemoveMeetingAt(meetingToRemove.StartTime);
            Console.WriteLine("Встреча успешно удалена.");
        }
        else
        {
            Console.WriteLine("Встреча не найдена.");
        }
    }

    static void UpdateMeeting(MeetingManager manager)
    {
        var oldTitle = GetInput("Введите название старой встречи: ");
        var oldMeeting = manager.GetMeetings(DateTime.Now).Find(m => m.Title == oldTitle);

        if (oldMeeting == null)
        {
            Console.WriteLine("Старая встреча не найдена.");
            return;
        }

        var newTitle = GetInput("Введите новое название встречи: ");
        var newStartTime = GetDateTimeInput("Введите новое время начала (гггг-мм-дд чч:мм): ");
        var newPlannedEndTime = GetDateTimeInput("Введите новое время окончания (гггг-мм-дд чч:мм): ");
        var newReminderTime = GetOptionalTimeSpanInput("Введите новое время напоминания (чч:мм) или оставьте пустым: ");
        
        var newMeeting = new Meeting(newTitle, newStartTime, newPlannedEndTime, newReminderTime);
        try
        {
            manager.UpdateMeeting(oldMeeting, newMeeting);
            Console.WriteLine("Встреча успешно обновлена.");
        }
        catch (Exception e)
        {
            WriteError(e.Message);
        }
    }

    static void ViewMeetings(MeetingManager manager)
    {
        var date = GetDateTimeInput("Введите дату для просмотра встреч (гггг-мм-дд): ");
        var meetings = manager.GetMeetings(date);

        if (meetings.Count == 0)
            Console.WriteLine("Встречи на указанную дату не найдены.");

        foreach (var meeting in meetings)
            Console.WriteLine($"{meeting.Title}: {meeting.StartTime} - {meeting.PlannedEndTime}");
    }

    static void ExportMeetings(MeetingManager manager)
    {
        var date = GetDateTimeInput("Введите дату для экспорта встреч (гггг-мм-дд): ");
        var filePath = GetInput("Введите путь для сохранения файла: ");

        manager.ExportMeetingsToFile(date, filePath);
        Console.WriteLine("Встречи успешно экспортированы.");
    }
}
