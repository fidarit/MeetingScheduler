namespace MeetingScheduler.Tests;


public class MeetingManagerTests
{
    private readonly MeetingManager _manager = new();

    [Fact]
    public void AddNormalMeeting()
    {
        var meeting = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        _manager.AddMeeting(meeting);

        var meetings = _manager.GetMeetings(new DateTime(2024, 10, 22));
        Assert.Single(meetings);
        Assert.Equal(meeting.Title, meetings[0].Title);
    }
    
    [Fact]
    public void AddConflictInTheEnd()
    {
        var meeting1 = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        _manager.AddMeeting(meeting1);

        var meeting2 = new Meeting("Встреча 2", new DateTime(2024, 10, 22, 10, 30, 0), new DateTime(2024, 10, 22, 11, 30, 0));

        CheckConflict(meeting2);
    }
    
    [Fact]
    public void AddConflictInTheBegin()
    {
        var meeting1 = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 11, 0, 0), new DateTime(2024, 10, 22, 12, 0, 0));
        _manager.AddMeeting(meeting1);

        var meeting2 = new Meeting("Встреча 2", new DateTime(2024, 10, 22, 10, 30, 0), new DateTime(2024, 10, 22, 11, 30, 0));

        CheckConflict(meeting2);
    }
    
    [Fact]
    public void AddConflictInTheMiddle()
    {
        var meeting1 = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        _manager.AddMeeting(meeting1);

        var meeting2 = new Meeting("Встреча 2", new DateTime(2024, 10, 22, 10, 15, 0), new DateTime(2024, 10, 22, 10, 45, 0));

        CheckConflict(meeting2);
    }
    
    [Fact]
    public void RemoveMeeting()
    {
        var meeting = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        _manager.AddMeeting(meeting);
        _manager.RemoveMeeting(meeting);

        var meetings = _manager.GetMeetings(new DateTime(2024, 10, 22));
        Assert.Empty(meetings);
    }
    
    [Fact]
    public void RemoveUnexistedMeeting()
    {
        var meeting = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));

        Assert.Throws<InvalidOperationException>(() => _manager.RemoveMeeting(meeting));
    }

    [Fact]
    public void UpdateMeeting()
    {
        var meeting = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        _manager.AddMeeting(meeting);

        var updatedMeeting = new Meeting("Обновленная встреча", new DateTime(2024, 10, 22, 11, 0, 0), new DateTime(2024, 10, 22, 12, 0, 0));
        _manager.UpdateMeeting(meeting, updatedMeeting);

        var meetings = _manager.GetMeetings(new DateTime(2024, 10, 22));
        Assert.Single(meetings);
        Assert.Equal(updatedMeeting.Title, meetings[0].Title);
    }
    
    [Fact]
    public void UpdateMeetingWithConflicted()
    {
        var meeting1 = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        _manager.AddMeeting(meeting1);

        var meeting2 = new Meeting("Встреча 2", new DateTime(2024, 10, 22, 10, 30, 0), new DateTime(2024, 10, 22, 11, 30, 0));

        CheckConflict(meeting2);
    }

    [Fact]
    public void GetMeetings()
    {
        var meeting1 = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        var meeting2 = new Meeting("Встреча 2", new DateTime(2024, 10, 22, 11, 0, 0), new DateTime(2024, 10, 22, 12, 0, 0));
        _manager.AddMeeting(meeting1);
        _manager.AddMeeting(meeting2);

        var meetings = _manager.GetMeetings(new DateTime(2024, 10, 22));
        Assert.Equal(2, meetings.Count);
    }

    [Fact]
    public void ExportMeetingsToFile()
    {
        var meeting = new Meeting("Встреча 1", new DateTime(2024, 10, 22, 10, 0, 0), new DateTime(2024, 10, 22, 11, 0, 0));
        _manager.AddMeeting(meeting);

        string filePath = Path.GetTempFileName();
        _manager.ExportMeetingsToFile(new DateTime(2024, 10, 22), filePath);

        var fileContent = File.ReadAllText(filePath);
        Assert.Contains($"{meeting.Title}: {meeting.StartTime} - {meeting.PlannedEndTime}", fileContent);

        File.Delete(filePath);
    }

    private void CheckConflict(Meeting meeting2)
    {
        var exception = Assert.Throws<InvalidOperationException>(() => _manager.AddMeeting(meeting2));
        Assert.Contains("конфликт", exception.Message);
    }
}