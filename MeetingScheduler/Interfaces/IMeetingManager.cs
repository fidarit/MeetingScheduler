using MeetingScheduler.Models;

namespace MeetingScheduler.Interfaces
{
    internal interface IMeetingManager
    {
        event EventHandler<MeetingEventArgs>? MeetingAdded;
        event EventHandler<MeetingEventArgs>? MeetingRemoved;
        event EventHandler<MeetingUpdateEventArgs>? MeetingUpdated;

        void AddMeeting(Meeting meeting);

        void ExportMeetingsToFile(DateTime date, string filePath);

        /// <summary>
        /// Возвращает встречи за указанную дату (время не учитывается)
        /// </summary>
        List<Meeting> GetMeetings(DateTime date);

        /// <summary>
        /// Возвращает встречи в указанном промежутке времени
        /// </summary>
        /// <param name="from">Дата\время, входит в промежуток</param>
        /// <param name="to">Дата\время, не входит в промежуток</param>
        List<Meeting> GetMeetings(DateTime from, DateTime to);

        void RemoveMeetingAt(DateTime startTime);

        void UpdateMeeting(Meeting oldMeeting, Meeting newMeeting);
    }
}