using FluentAssertions;
using NSubstitute;
using ScheduleService.Application;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.TypeSchedules;
using ScheduleService.Domain.ValueObjects;
using SharedKernel;

namespace ScheduleService.Domain.Tests;

public class WeeklyScheduleTests
{
    private readonly ScheduleId _scheduleId = ScheduleId.New();
    private readonly TimeSpan _duration = new(18, 0, 0);
    private readonly Title _title = Title.Create("Daily Schedule").Value;
    private readonly Description _description = Description.Create("Test description").Value;
    private readonly IDateTimeProvider _dateTimeProvider;

    public WeeklyScheduleTests()
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        dateTimeProvider.UtcNow.Returns(fixedDate);
        dateTimeProvider.Now.Returns(fixedDate.ToLocalTime());
        _dateTimeProvider = dateTimeProvider;
    }
    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(14);
        var executionDays = new List<EventTime>
        {
            EventTime.Create(DayOfWeek.Monday, TimeSpan.FromHours(10)).Value,
            EventTime.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(15)).Value
        };
        var repeatInterval = 2;
        var isAutomaticRenewal = true;

        // Act
        var result = WeeklySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            _duration, 
            executionDays, 
            _title, 
            _description,
            repeatInterval, 
            isAutomaticRenewal);

        // Assert

        //  расписание было запланировано на endDate (2 недели)
        var countWeeksScheduleActive = 2;
        var countExpectedEvents = countWeeksScheduleActive * executionDays.Count;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PlannedEvents.Should().HaveCount(countExpectedEvents);
        result.Value.RepeatInterval.Should().Be(repeatInterval);
        result.Value.ExecutionDays.Should().BeEquivalentTo(executionDays);
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldReturnError()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(-7);
        var executionDays = new List<EventTime> { EventTime.Create(DayOfWeek.Monday, TimeSpan.FromHours(10)).Value };
        var repeatInterval = 1;
        var isAutomaticRenewal = true;

        // Act
        var result = WeeklySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            _duration, 
            executionDays, 
            _title, 
            _description,
            repeatInterval, 
            isAutomaticRenewal);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.ValueIsInvalid("endDate"));
    }

    [Fact]
    public void Create_WithEmptyExecutionDays_ShouldReturnError()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(14);
        var executionDays = new List<EventTime>();
        var repeatInterval = 1;
        var isAutomaticRenewal = true;

        // Act
        var result = WeeklySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            _duration, 
            executionDays, 
            _title, 
            _description,
            repeatInterval, 
            isAutomaticRenewal);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.ValueIsInvalid("executionTimeList"));
    }

    [Fact]
    public void ExtendEvents_WithAutomaticRenewal_ShouldAddNewEvents()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(14);
        var executionDays = new List<EventTime>
        {
            EventTime.Create(DayOfWeek.Monday, TimeSpan.FromHours(10)).Value,
            EventTime.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(15)).Value
        };
        var repeatInterval = 2;
        var isAutomaticRenewal = true;

        var schedule = WeeklySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            _duration, 
            executionDays, 
            _title, 
            _description,
            repeatInterval, 
            isAutomaticRenewal).Value;

        // Act
        var result = schedule.ExtendEvents();

        // Assert

        //  расписание было запланировано на repeatInterval (2 недели) + продление на неделю вперед
        var countWeeksScheduleActive = 3;
        var countExpectedEvents = countWeeksScheduleActive * executionDays.Count;
        result.IsSuccess.Should().BeTrue();
        schedule.PlannedEvents.Should().NotBeEmpty();
        schedule.PlannedEvents.Should().HaveCount(countExpectedEvents);
    }

    [Fact]
    public void ExtendEvents_WithAutomaticRenewalDisabled_ShouldReturnError()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(14);
        var executionDays = new List<EventTime> { EventTime.Create(DayOfWeek.Monday, TimeSpan.FromHours(10)).Value };
        var repeatInterval = 2;
        var isAutomaticRenewal = false;

        var schedule = WeeklySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            _duration, 
            executionDays, 
            _title, 
            _description,
            repeatInterval, 
            isAutomaticRenewal).Value;

        // Act
        var result = schedule.ExtendEvents();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ErrorsSchedule.Schedule.RenewalIsDisabled());
    }
}
