namespace MeetingScheduler;


internal record Meeting(string Title, DateTime StartTime, DateTime PlannedEndTime, TimeSpan? ReminderTime = null);


internal class MeetingManager
{
    public void AddMeeting(Meeting meeting)
    {
        if (MeetingHasConflicts(meeting))
            throw new InvalidOperationException("Встреча конфликтует с существующей встречей.");

        throw new NotImplementedException();
    }

    public void RemoveMeeting(Meeting meeting)
    {
        throw new NotImplementedException();
    }

    public void UpdateMeeting(Meeting oldMeeting, Meeting newMeeting)
    {
        RemoveMeeting(oldMeeting);
        AddMeeting(newMeeting);
    }

    public List<Meeting> GetMeetings(DateTime date)
    {
        throw new NotImplementedException();
    }

    public void ExportMeetingsToFile(DateTime date, string filePath)
    {
        throw new NotImplementedException();
    }


    private bool MeetingHasConflicts(Meeting newMeeting)
    {
        throw new NotImplementedException();
    }
}
