using FluentAssertions;
using NSubstitute;
using ScheduleService.Application;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.Ids;

namespace ScheduleService.Domain.Tests;

public class EventInstaceTests
{
    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly EventInstance _eventInstance;

    public EventInstaceTests()
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        dateTimeProvider.UtcNow.Returns(fixedDate);
        dateTimeProvider.Now.Returns(fixedDate.ToLocalTime());
        _dateTimeProvider = dateTimeProvider;

        _eventInstance = new(EventId.New(),
            dateTimeProvider.UtcNow,
            ScheduleId.New(),
            new TimeSpan(16, 0, 0),
            EventStatus.Scheduled);
    }

    [Fact]
    public void Create_successfully_event_instance()
    {
        // arrange
        var eventId = EventId.New();
        var scheduleId = ScheduleId.New();
        var start = _dateTimeProvider.UtcNow;
        var status = EventStatus.Scheduled;
        var durationEvent = new TimeSpan(18, 0, 0);

        // act
        var result = new EventInstance(eventId, start, scheduleId, durationEvent, status);

        // assert
        result.Id.Should().Be(eventId);
        result.Start.Should().Be(start);
        result.Duration.Should().Be(durationEvent);
        result.ScheduleId.Should().Be(scheduleId);
        result.Status.Should().Be(status);
    }

    [Fact]
    public void Reschedule_successfully_event_instance()
    {
        // arrange
        var newDate = _dateTimeProvider.UtcNow.AddDays(1);

        // act
        _eventInstance.Reschedule(newDate);

        // assert
        _eventInstance.Start.Should().Be(newDate);
        _eventInstance.Status.Should().Be(EventStatus.Rescheduled);
    }

    [Fact]
    public void Cancel_successfully_event_instance()
    {
        // act
        _eventInstance.Cancel();

        // assert
        _eventInstance.Status.Should().Be(EventStatus.Cancelled);
    }

    [Fact]
    public void Finish_successfully_event_instance()
    {
        // act
        _eventInstance.Finish();

        // assert
        _eventInstance.Status.Should().Be(EventStatus.Finished);
    }

    [Fact]
    public void SetDuration_successfully_event_instance()
    {
        // arrange
        var duration = TimeSpan.FromMinutes(30);

        // act
        _eventInstance.SetDuration(duration);

        // assert
        _eventInstance.Duration.Should().Be(duration);
    }
}
