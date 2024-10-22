using MeetingScheduler.Services;


namespace MeetingScheduler.Tests;


public class MeetingManagerTests
{
    private readonly MeetingManager _manager = new();

    #region Add
    [Fact]
    public void AddNormalMeeting()
    {
        var startDate = GetValidDateTime();
        var meeting = new Meeting("Встреча 1", startDate, startDate.AddHours(1));
        _manager.AddMeeting(meeting);

        var meetings = _manager.GetMeetings(startDate.Date);
        Assert.Single(meetings);
        Assert.Equal(meeting.Title, meetings[0].Title);
    }
    
    [Fact]
    public void AddLateMeeting()
    {
        var startDate = DateTime.Now.AddDays(-1);
        var meeting = new Meeting("Встреча 1", startDate, startDate.AddHours(1));

        Assert.Throws<ArgumentOutOfRangeException>(() => _manager.AddMeeting(meeting));
        var meetings = _manager.GetMeetings(startDate.Date);
        Assert.Empty(meetings);
    }

    [Fact]
    public void AddConflictInTheEnd()
    {
        var startDate1 = GetValidDateTime();
        var meeting1 = new Meeting("Встреча 1", startDate1, startDate1.AddHours(1));
        _manager.AddMeeting(meeting1);

        var startDate2 = startDate1.AddMinutes(30);
        var meeting2 = new Meeting("Встреча 2", startDate2, startDate2.AddHours(1));

        CheckConflict(meeting2);
    }

    [Fact]
    public void AddConflictInTheBegin()
    {
        var startDate1 = GetValidDateTime();
        var meeting1 = new Meeting("Встреча 1", startDate1, startDate1.AddHours(1));
        _manager.AddMeeting(meeting1);

        var startDate2 = startDate1.AddMinutes(-30);
        var meeting2 = new Meeting("Встреча 2", startDate2, startDate2.AddHours(1));

        CheckConflict(meeting2);
    }

    [Fact]
    public void AddConflictInTheMiddle()
    {
        var startDate1 = GetValidDateTime();
        var meeting1 = new Meeting("Встреча 1", startDate1, startDate1.AddHours(1));
        _manager.AddMeeting(meeting1);

        var startDate2 = startDate1.AddMinutes(15);
        var meeting2 = new Meeting("Встреча 2", startDate2, startDate2.AddMinutes(30));

        CheckConflict(meeting2);
    }
    #endregion

    #region Remove
    [Fact]
    public void RemoveMeeting()
    {
        var startDate = GetValidDateTime();
        var meeting = new Meeting("Встреча 1", startDate, startDate.AddHours(1));
        _manager.AddMeeting(meeting);
        _manager.RemoveMeetingAt(startDate);

        var meetings = _manager.GetMeetings(startDate.Date);
        Assert.Empty(meetings);
    }

    [Fact]
    public void RemoveUnexistedMeeting()
    {
        var startDate = GetValidDateTime();
        var meeting = new Meeting("Встреча 1", startDate, startDate.AddHours(1));

        Assert.Throws<InvalidOperationException>(() => _manager.RemoveMeetingAt(meeting.StartTime));
    }
    #endregion

    #region Update
    [Fact]
    public void UpdateMeeting()
    {
        var startDate= GetValidDateTime();
        var meeting = new Meeting("Встреча 1", startDate, startDate.AddHours(1));
        _manager.AddMeeting(meeting);

        startDate = startDate.AddHours(1);
        var updatedMeeting = new Meeting("Обновленная встреча", startDate, startDate.AddHours(1));
        _manager.UpdateMeeting(meeting, updatedMeeting);

        var meetings = _manager.GetMeetings(startDate.Date);
        Assert.Single(meetings);
        Assert.Equal(updatedMeeting.Title, meetings[0].Title);
    }

    [Fact]
    public void UpdateMeetingWithConflicted()
    {
        var startDate1 = GetValidDateTime();
        var startDate2 = startDate1.AddHours(1);
        var conflictDate = startDate1.AddHours(0.5);

        var meeting1 = new Meeting("Встреча 1", startDate1, startDate1.AddHours(1));
        var meeting2 = new Meeting("Встреча 2", startDate2, startDate2.AddHours(1));
        _manager.AddMeeting(meeting1);
        _manager.AddMeeting(meeting2);

        var meeting1_updated = new Meeting("Встреча 1 обновленная", conflictDate, conflictDate.AddHours(1));

        CheckConflict(() => _manager.UpdateMeeting(meeting1, meeting1_updated));

        var meetings = _manager.GetMeetings(startDate1.Date);
        Assert.Equal(2, meetings.Count);
        Assert.Equal(meeting1.Title, meetings[0].Title);
        Assert.Equal(meeting2.Title, meetings[1].Title);
    }
    #endregion

    #region Other
    [Fact]
    public void GetMeetings()
    {
        var sameDay1 = GetValidDateTime();
        var sameDay2 = sameDay1.AddHours(2);
        var nextDay3 = sameDay1.AddDays(1).Date;

        var sameDayMeeting1 = new Meeting("Встреча 1", sameDay1, sameDay1.AddHours(1));
        var sameDayMeeting2 = new Meeting("Встреча 2", sameDay2, sameDay2.AddHours(1));
        var nextDayMeeting = new Meeting("Встреча 3", nextDay3, nextDay3.AddHours(1));
        _manager.AddMeeting(sameDayMeeting1);
        _manager.AddMeeting(sameDayMeeting2);
        _manager.AddMeeting(nextDayMeeting);

        var sameDayMeetings = _manager.GetMeetings(sameDay1.Date);
        Assert.Equal(2, sameDayMeetings.Count);
        Assert.Equal(sameDayMeeting1, sameDayMeetings[0]);
        Assert.Equal(sameDayMeeting2, sameDayMeetings[1]);

        var nextDayMeetings = _manager.GetMeetings(nextDay3.Date);
        Assert.Single(nextDayMeetings);
        Assert.Equal(nextDayMeeting, nextDayMeetings[0]);
    }

    [Fact]
    public void ExportMeetingsToFile()
    {
        var startDate = GetValidDateTime();
        var meeting = new Meeting("Встреча 1", startDate, startDate.AddHours(1));
        _manager.AddMeeting(meeting);

        string filePath = Path.GetTempFileName();
        _manager.ExportMeetingsToFile(startDate.Date, filePath);

        var fileContent = File.ReadAllText(filePath);
        Assert.Contains($"{meeting.Title}: {meeting.StartTime} - {meeting.PlannedEndTime}", fileContent);

        File.Delete(filePath);
    }
    #endregion

    #region Private
    private void CheckConflict(Meeting meeting2)
    {
        CheckConflict(() => _manager.AddMeeting(meeting2));
    }    

    private void CheckConflict(Action action)
    {
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Contains("конфликт", exception.Message);
    }

    private DateTime GetValidDateTime() => DateTime.Now.AddDays(1).Date.AddHours(12);
    #endregion
}