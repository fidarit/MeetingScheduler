using MeetingScheduler.Tools;


namespace MeetingScheduler.Services;


internal record Meeting(string Title, DateTime StartTime, DateTime PlannedEndTime, TimeSpan? ReminderTime = null);


internal class MeetingManager
{
    /// <summary>
    /// StartTime: Meeting
    /// </summary>
    private readonly SortedList<DateTime, Meeting> meetings = [];

    public void AddMeeting(Meeting meeting)
    {
        if (meeting.StartTime < DateTime.Now)
            throw new ArgumentOutOfRangeException(nameof(meeting.StartTime), "Встречи всегда планируются только на будущее время.");

        if (MeetingHasConflicts(meeting))
            throw new InvalidOperationException("Встреча конфликтует с существующей встречей.");

        meetings.Add(meeting.StartTime, meeting);
    }

    public void RemoveMeetingAt(DateTime startTime)
    {
        if (!meetings.Remove(startTime))
            throw new InvalidOperationException("Встреча не найдена.");
    }

    public void UpdateMeeting(Meeting oldMeeting, Meeting newMeeting)
    {
        RemoveMeetingAt(oldMeeting.StartTime);

        try
        {
            AddMeeting(newMeeting);
        }
        catch (InvalidOperationException)
        {
            meetings.Add(oldMeeting.StartTime, oldMeeting);
            throw;
        }
    }

    public List<Meeting> GetMeetings(DateTime date)
    {
        return meetings.Values
            .Where(m => m.StartTime.Date == date.Date)
            .ToList();
    }

    public void ExportMeetingsToFile(DateTime date, string filePath)
    {
        var meetingsForDate = GetMeetings(date);
        using var writer = new StreamWriter(filePath);

        foreach (var meeting in meetingsForDate)
        {
            writer.WriteLine($"{meeting.Title}: {meeting.StartTime} - {meeting.PlannedEndTime}");
        }
    }


    private bool MeetingHasConflicts(Meeting newMeeting)
    {
        if (meetings.ContainsKey(newMeeting.StartTime))
            return true;

        var index = meetings.BinarySearch(newMeeting.StartTime);
        if (index < 0)
            index = ~index;

        if (index > 0)
        {
            var previousMeeting = meetings.Values[index - 1];
            if (newMeeting.StartTime < previousMeeting.PlannedEndTime)
                return true;
        }

        if (index < meetings.Count)
        {
            var nextMeeting = meetings.Values[index];
            if (newMeeting.PlannedEndTime > nextMeeting.StartTime)
                return true;
        }

        return false;
    }
}
