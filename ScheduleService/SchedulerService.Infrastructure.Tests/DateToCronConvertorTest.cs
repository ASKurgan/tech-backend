using NSubstitute;
using ScheduleService.Application;
using ScheduleService.Infrastructure.QuartzManagement;

namespace SchedulerService.Infrastructure.Tests;

public class DateToCronConvertorTest
{
    [Fact]
    public void GetDailyCron_ItShould_Return_Right_Expression()
    {
        //Arrange 
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();        
        dateTimeProvider.UtcNow.Returns(
            new DateTime(2024, 11, 5, 15, 35, 0, DateTimeKind.Utc));
        var testExpression = "0 35 15 1/1 * ? *";

        //Act
        var result = DateToCronConvertor.GetDailyCron(dateTimeProvider.UtcNow);

        //Assert
        Assert.Equal(testExpression, result);
    }

    [Fact]
    public void GetWeeklyCron_ItShould_Return_Right_Expression()
    {
        //Arrange 
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(
            new DateTime(2024, 11, 5, 15, 35, 0, DateTimeKind.Utc));

        DayOfWeek dayOfWeek = DayOfWeek.Monday;

        var testExpression = $"0 35 15 ? * {(int)dayOfWeek + 1} *";

        //Act
        var result = DateToCronConvertor.GetWeeklyCron(dateTimeProvider.UtcNow, dayOfWeek);

        //Assert
        Assert.Equal(testExpression, result);
    }

    [Fact]
    public void GetMonthlyCron_ItShould_Return_Right_Expression()
    {
        //Arrange 
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(
            new DateTime(2024, 11, 5, 15, 35, 0, DateTimeKind.Utc));

        var testExpression = $"0 35 15 5 1/1 ? *";

        //Act
        var result = DateToCronConvertor.GetMonthlyCron(dateTimeProvider.UtcNow);

        //Assert
        Assert.Equal(testExpression, result);
    }
}