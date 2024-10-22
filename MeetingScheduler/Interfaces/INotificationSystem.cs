using MeetingScheduler.Models;

namespace MeetingScheduler.Interfaces
{
    internal interface INotificationSystem : IDisposable
    {
        event EventHandler<MeetingEventArgs>? OnNotification;
    }
}