namespace MeetingScheduler.Models;

internal record Meeting(string Title, DateTime StartTime, DateTime PlannedEndTime, TimeSpan? ReminderTime = null);
