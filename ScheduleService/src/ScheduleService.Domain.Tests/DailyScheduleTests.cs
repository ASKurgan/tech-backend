using FluentAssertions;
using NSubstitute;
using ScheduleService.Application;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.TypeSchedules;
using ScheduleService.Domain.ValueObjects;

namespace ScheduleService.Domain.Tests;

public class DailyScheduleTests
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ScheduleId _scheduleId = ScheduleId.New();
    private readonly TimeSpan _duration = new(18, 0, 0); 
    private readonly Title _title = Title.Create("Daily Schedule").Value;
    private readonly Description _description = Description.Create("Test description").Value;

    public DailyScheduleTests()
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        dateTimeProvider.UtcNow.Returns(fixedDate);
        dateTimeProvider.Now.Returns(fixedDate.ToLocalTime());
        _dateTimeProvider = dateTimeProvider;
    }
    
    [Fact]
    public void Create_ShouldReturnSuccess_WhenValidInput()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate =  _dateTimeProvider.UtcNow.AddDays(10);
        var executionTimes = new List<TimeSpan> { new(18, 0, 0)};
        var isAutomaticRenewal = true;

        // Act
        var result = DailySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            executionTimes, 
            _duration, 
            _title, 
            _description,
            isAutomaticRenewal);

        // Assert
        var countExpectedEvents = (endDate - startDate).Days;
        result.IsSuccess.Should().BeTrue();
        result.Value.ExecutionTimes.Should().BeEquivalentTo(executionTimes);
        result.Value.PlannedEvents.Count.Should().Be(countExpectedEvents);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenExecutionTimesAreEmpty()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(10);
        var executionTimes = new List<TimeSpan>(); // пустой список
        var isAutomaticRenewal = true;

        // Act
        var result = DailySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            executionTimes, 
            _duration, 
            _title, 
            _description,
            isAutomaticRenewal);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.ValueIsInvalid("executionTimeList"));
    }

    [Fact]
    public void Create_ShouldReturnError_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow.AddDays(10);
        var endDate = _dateTimeProvider.UtcNow; 
        var executionTimes = new List<TimeSpan> { TimeSpan.FromHours(9), TimeSpan.FromHours(15) };
        var isAutomaticRenewal = true;

        // Act
        var result = DailySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            executionTimes, 
            _duration, 
            _title, 
            _description,
            isAutomaticRenewal);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.ValueIsInvalid("endDate"));
    }

    [Fact]
    public void ExtendEvents_ShouldAddNewEvents_WhenAutomaticRenewalEnabled()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(10);
        var executionTimes = new List<TimeSpan> { TimeSpan.FromHours(9) };
        var isAutomaticRenewal = true;

        var schedule = DailySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            executionTimes, 
            _duration, 
            _title, 
            _description,
            isAutomaticRenewal).Value;

        // Act
        var result = schedule.ExtendEvents();

        // Assert
        var countExpectedEvents = (endDate - startDate).Days;
        result.IsSuccess.Should().BeTrue();
        schedule.PlannedEvents.Should().HaveCount(countExpectedEvents + 1);
    }

    [Fact]
    public void ExtendEvents_ShouldReturnError_WhenAutomaticRenewalDisabled()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddDays(10);
        var executionTimes = new List<TimeSpan> { TimeSpan.FromHours(9), TimeSpan.FromHours(15) };
        var isAutomaticRenewal = false;

        var schedule = DailySchedule.Create(_scheduleId, 
            startDate, 
            endDate, 
            executionTimes, 
            _duration, 
            _title, 
            _description,
            isAutomaticRenewal).Value;

        // Act
        var result = schedule.ExtendEvents();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.Schedule.RenewalIsDisabled());
    }
}
