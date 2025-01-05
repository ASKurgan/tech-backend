using FluentAssertions;
using NSubstitute;
using ScheduleService.Application;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.TypeSchedules;
using ScheduleService.Domain.ValueObjects;
using SharedKernel;

namespace ScheduleService.Domain.Tests;

public class MonthlyScheduleTests
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ScheduleId _scheduleId = ScheduleId.New();
    private readonly TimeSpan _duration = new(18, 0, 0);
    private readonly Title _title = Title.Create("Daily Schedule").Value;
    private readonly Description _description = Description.Create("Test description").Value;

    public MonthlyScheduleTests()
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
        var endDate = startDate.AddMonths(2);
        var executionDays = new List<DateTime> { startDate.AddDays(5) };
        var repeatInterval = 2;
        var isAutomaticRenewal = true;

        // Act
        var result = MonthlySchedule.Create(_scheduleId,
            startDate,
            endDate,
            executionDays,
            _duration,
            _title,
            _description,
            repeatInterval,
            isAutomaticRenewal);

        // Assert

        //  расписание было запланировано на endDate (2 месяца)
        var countMonthsScheduleActive = 2;
        var countExpectedEvents = countMonthsScheduleActive * executionDays.Count;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.RepeatInterval.Should().Be(repeatInterval);
        result.Value.ExecutionDays.Should().BeEquivalentTo(executionDays);
        result.Value.PlannedEvents.Should().HaveCount(countExpectedEvents);
    }

    [Fact]
    public void Create_WithEmptyExecutionDays_ShouldReturnError()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddMonths(2);
        var executionDays = new List<DateTime>();
        var repeatInterval = 2;
        var isAutomaticRenewal = true;

        // Act
        var result = MonthlySchedule.Create(_scheduleId,
            startDate,
            endDate,
            executionDays,
            _duration,
            _title,
            _description,
            repeatInterval,
            isAutomaticRenewal);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.ValueIsInvalid("executionTimeList"));
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldReturnError()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddMonths(-1);
        var executionDays = new List<DateTime> { startDate.AddDays(5) };
        var repeatInterval = 2;
        var isAutomaticRenewal = true;

        // Act
        var result = MonthlySchedule.Create(_scheduleId,
            startDate,
            endDate,
            executionDays,
            _duration,
            _title,
            _description,
            repeatInterval,
            isAutomaticRenewal);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.General.ValueIsInvalid("endDate"));
    }

    [Fact]
    public void ExtendEvents_WithAutomaticRenewal_ShouldAddNewEvents()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddMonths(1);
        var executionDays = new List<DateTime> { startDate.AddDays(5), startDate.AddDays(15) };
        var repeatInterval = 1;
        var isAutomaticRenewal = true;

        var schedule = MonthlySchedule.Create(_scheduleId,
            startDate,
            endDate,
            executionDays,
            _duration,
            _title,
            _description,
            repeatInterval,
            isAutomaticRenewal).Value;

        // Act
        var result = schedule.ExtendEvents();

        // Assert
        //  расписание было запланировано на endDate (1 месяц) + продление на 1 месяц
        var countMonthsScheduleActive = 2;
        var countExpectedEvents = countMonthsScheduleActive * executionDays.Count;

        result.IsSuccess.Should().BeTrue();
        schedule.PlannedEvents.Should().HaveCount(countExpectedEvents);
    }

    [Fact]
    public void ExtendEvents_WithAutomaticRenewalDisabled_ShouldReturnError()
    {
        // Arrange
        var startDate = _dateTimeProvider.UtcNow;
        var endDate = startDate.AddMonths(1);
        var executionDays = new List<DateTime> { startDate.AddDays(5), startDate.AddDays(15) };
        var repeatInterval = 1;
        var isAutomaticRenewal = false;

        var schedule = MonthlySchedule.Create(_scheduleId,
            startDate,
            endDate,
            executionDays,
            _duration,
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

    [Theory]
    [InlineData(2024, 2, 28, 2024, 3, 28)] // Leap year February
    [InlineData(2023, 1, 31, 2023, 2, 28)] // 31st in February
    [InlineData(2024, 6, 15, 2024, 7, 15)] // Normal day in month
    public void CalculateMonthlyDate_WithValidInputs_ShouldReturnCorrectDate(
        int startYear, int startMonth, int startDay,
        int expectedYear, int expectedMonth, int expectedDay)
    {
        // Arrange
        var startDate = new DateTime(startYear, startMonth, startDay);
        var executionDay = new DateTime(1, 1, startDay, 10, 0, 0); // Day only for calculation

        // Act
        var result = MonthlySchedule.CalculateMonthlyDate(startDate, 1, executionDay);

        // Assert
        result.Year.Should().Be(expectedYear);
        result.Month.Should().Be(expectedMonth);
        result.Day.Should().Be(expectedDay);
    }
}
