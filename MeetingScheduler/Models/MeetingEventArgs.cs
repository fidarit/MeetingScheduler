namespace MeetingScheduler.Models;


internal class MeetingEventArgs(Meeting meeting) : EventArgs
{
    public Meeting Meeting { get; } = meeting;
}


internal class MeetingUpdateEventArgs(Meeting oldMeeting, Meeting newMeeting) : EventArgs
{
    public Meeting OldMeeting { get; } = oldMeeting;
    public Meeting NewMeeting { get; } = newMeeting;
}

