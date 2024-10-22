using MeetingScheduler.Interfaces;
using MeetingScheduler.Models;

namespace MeetingScheduler.Services;

internal class NotificationSystem : INotificationSystem, IDisposable
{
    private readonly IMeetingManager meetingManager;
    private readonly Dictionary<Meeting, CancellationTokenSource> CancellationTokenSources = [];

    public event EventHandler<MeetingEventArgs>? OnNotification;

    public NotificationSystem(IMeetingManager meetingManager)
    {
        this.meetingManager = meetingManager;

        meetingManager.MeetingAdded += OnMeetingAdded;
        meetingManager.MeetingRemoved += OnMeetingRemoved;
        meetingManager.MeetingUpdated += OnMeetingUpdated;
    }

    public void Dispose()
    {
        meetingManager.MeetingAdded -= OnMeetingAdded;
        meetingManager.MeetingRemoved -= OnMeetingRemoved;
        meetingManager.MeetingUpdated -= OnMeetingUpdated;

        var meetings = CancellationTokenSources.Keys.ToArray();
        foreach (var meeting in meetings)
            RemoveListenerTaskOf(meeting);
    }

    private void OnMeetingAdded(object? sender, MeetingEventArgs e)
    {
        AddListenerTaskFor(e.Meeting);
    }

    private void OnMeetingRemoved(object? sender, MeetingEventArgs e)
    {
        RemoveListenerTaskOf(e.Meeting);
    }

    private void OnMeetingUpdated(object? sender, MeetingUpdateEventArgs e)
    {
        RemoveListenerTaskOf(e.OldMeeting);
        AddListenerTaskFor(e.NewMeeting);
    }

    private void AddListenerTaskFor(Meeting meeting)
    {
        if (meeting.ReminderTime == null)
            return;

        if (CancellationTokenSources.ContainsKey(meeting))
            return;

        var reminderTime = meeting.StartTime - meeting.ReminderTime.Value;
        var delay = reminderTime - DateTime.Now;

        if (delay.TotalMilliseconds > 0)
        {
            var cancelationTokenSource = new CancellationTokenSource();
            Task.Delay(delay, cancelationTokenSource.Token).ContinueWith(_ => Notify(meeting));
            CancellationTokenSources[meeting] = cancelationTokenSource;
        }
        else
            Notify(meeting);
    }

    private void RemoveListenerTaskOf(Meeting meeting, bool cancelTask = true)
    {
        if (!CancellationTokenSources.TryGetValue(meeting, out var cancelationTokenSource))
            return;

        if (cancelTask)
            cancelationTokenSource.Cancel();

        cancelationTokenSource.Dispose();
        CancellationTokenSources.Remove(meeting);
    }

    private void Notify(Meeting meeting)
    {
        OnNotification?.Invoke(this, new(meeting));

        RemoveListenerTaskOf(meeting, false);

        
    }
}
