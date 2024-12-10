using FluentAssertions;
using NSubstitute;
using ScheduleService.Application;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.ValueObjects;

namespace ScheduleService.Domain.Tests;

public class ScheduleTests
{
    private class SimpleScheduleTest : Schedule
    {
        public SimpleScheduleTest(
            ScheduleId id, Title title, Description description, DateTime startDate, DateTime endDate,
            TimeSpan eventDuration, bool isAutomaticRenewal) : base(id, title, description, startDate, endDate,
            eventDuration, isAutomaticRenewal)
        {
        }
    }

    public ScheduleTests()
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        dateTimeProvider.UtcNow.Returns(fixedDate);
        dateTimeProvider.Now.Returns(fixedDate.ToLocalTime());
        _dateTimeProvider = dateTimeProvider;

        var scheduleId = ScheduleId.New();
        
        _schedule = new SimpleScheduleTest(scheduleId,
            Title.Create("Test Schedule").Value,
            Description.Create("Description").Value,
            dateTimeProvider.UtcNow,
            dateTimeProvider.UtcNow.AddDays(10),
            TimeSpan.FromHours(1),
            true);
        
        _eventInstance = new EventInstance(EventId.New(),
            dateTimeProvider.UtcNow.AddDays(1),
            scheduleId,
            TimeSpan.FromHours(1),
            EventStatus.Scheduled);
        
    }

    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly SimpleScheduleTest _schedule;

    private readonly EventInstance _eventInstance;

    [Fact]
    public void AddEvent_ShouldAddEvent_WhenValid()
    {
        // Arrange
        // Act
        var result = _schedule.AddEvent(_eventInstance);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _schedule.PlannedEvents.Should().Contain(_eventInstance);
    }

    [Fact]
    public void AddEvent_ShouldReturnError_WhenEventAlreadyExists()
    {
        // Arrange
        _schedule.AddEvent(_eventInstance);

        // Act
        var result = _schedule.AddEvent(_eventInstance);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.AlreadyExist());
    }

    [Fact]
    public void RescheduleEvent_ShouldUpdateEventDate_WhenEventExists()
    {
        // Arrange
        var __eventInstance = new EventInstance(EventId.New(),
            _dateTimeProvider.UtcNow.AddDays(1),
            _schedule.Id,
            TimeSpan.FromHours(1),
            EventStatus.Scheduled);

        _schedule.AddEvent(__eventInstance);

        var newDate = _dateTimeProvider.UtcNow.AddDays(2);

        // Act
        var result = _schedule.RescheduleEvent(__eventInstance, newDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _schedule.PlannedEvents.Should().Contain(e => e.Id == __eventInstance.Id && e.Start == newDate);
    }

    [Fact]
    public void RescheduleEvent_ShouldReturnError_WhenEventDoesNotExist()
    {
        // Arrange
        var newDate = _dateTimeProvider.UtcNow.AddDays(2);

        // Act
        var result = _schedule.RescheduleEvent(_eventInstance, newDate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.NotFound(_eventInstance.Id.Value, "event instance"));
    }

    [Fact]
    public void RemoveEvent_ShouldRemoveEvent_WhenEventExists()
    {
        // Arrange
        _schedule.AddEvent(_eventInstance);

        // Act
        var result = _schedule.RemoveEvent(_eventInstance);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _schedule.PlannedEvents.Should().NotContain(_eventInstance);
    }

    [Fact]
    public void RemoveEvent_ShouldReturnError_WhenEventDoesNotExist()
    {
        // Act
        var result = _schedule.RemoveEvent(_eventInstance);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.NotFound(_eventInstance.Id.Value, "event instance"));
    }

    [Fact]
    public void UnableAutomaticRenewal_ShouldEnableRenewal_WhenDisabled()
    {
        // Arrange
        _schedule.DisableAutomaticRenewal();

        // Act
        var result = _schedule.UnableAutomaticRenewal();

        // Assert
        result.IsSuccess.Should().BeTrue();
        _schedule.IsAutomaticRenewal.Should().BeTrue();
    }

    [Fact]
    public void UnableAutomaticRenewal_ShouldReturnError_WhenAlreadyEnabled()
    {
        // Arrange
        _schedule.UnableAutomaticRenewal();

        // Act
        var result = _schedule.UnableAutomaticRenewal();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.Schedule.RenewalIsEnabled());
    }

    [Fact]
    public void DisableAutomaticRenewal_ShouldDisableRenewal_WhenEnabled()
    {
        // Arrange
        _schedule.UnableAutomaticRenewal();

        // Act
        var result = _schedule.DisableAutomaticRenewal();

        // Assert
        result.IsSuccess.Should().BeTrue();
        _schedule.IsAutomaticRenewal.Should().BeFalse();
    }

    [Fact]
    public void ChangeDurationForAllEvents_ShouldUpdateEventDurations()
    {
        // Arrange
        _schedule.AddEvent(_eventInstance);
        var newDuration = TimeSpan.FromHours(2);

        // Act
        _schedule.ChangeDurationForAllEvents(newDuration);

        // Assert
        _schedule.PlannedEvents.Should().AllSatisfy(e => e.Duration.Should().Be(newDuration));
    }

    [Fact]
    public void UpdateSchedule_ShouldChangeTitleAndDescription()
    {
        // Arrange
        var newTitle = Title.Create("Updated Title").Value;
        var newDescription = Description.Create("Updated Description").Value;

        // Act
        _schedule.UpdateSchedule(newTitle, newDescription);

        // Assert
        _schedule.Title.Should().Be(newTitle);
        _schedule.Description.Should().Be(newDescription);
    }
}
