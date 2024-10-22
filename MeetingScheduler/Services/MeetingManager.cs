using MeetingScheduler.Interfaces;
using MeetingScheduler.Models;
using MeetingScheduler.Tools;


namespace MeetingScheduler.Services;


internal class MeetingManager : IMeetingManager
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
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        return GetMeetings(startDate, endDate);
    }

    public List<Meeting> GetMeetings(DateTime from, DateTime to)
    {
        var startIndex = meetings.BinarySearch(from);
        var endIndex = meetings.BinarySearch(to);

        if (startIndex < 0)
            startIndex = ~startIndex;

        if (endIndex < 0)
            endIndex = ~endIndex;

        var count = endIndex - startIndex; //endIndex не входит в диапазон 
        if (count <= 0)
            return [];

        var result = new List<Meeting>(count);

        for (var i = startIndex; i < endIndex; i++)
            result.Add(meetings.GetValueAtIndex(i));

        return result;
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
