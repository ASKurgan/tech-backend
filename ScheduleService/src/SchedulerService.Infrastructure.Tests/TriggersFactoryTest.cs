using Quartz;
using ScheduleService.Infrastructure.QuartzManagement.Triggers;

namespace SchedulerService.Infrastructure.Tests;

public class TriggersFactoryTest
{
    [Fact]
    public void GetKey_ItShould_Return_Right_TriggerKey_Value()
    {
        //Arrange 
        var triggerName = "triggerKey";
        var group = "groupName";

        //Act
        var result = TriggersFactory.GetKey(triggerName, group);

        //Assert
        Assert.True(result is TriggerKey);
        Assert.Equal(triggerName, result.Name);
        Assert.Equal(group, result.Group);
    }

    [Fact]
    public void CreateWithCron_ItShould_Return_Right_TriggerKey_Value_For_TriggerKey()
    {
        //Arrange 
        var triggerName = "triggerKey";
        var group = "groupName";

        var triggerKey = TriggersFactory.GetKey(triggerName, group);
        var cron = "0 35 15 1/1 * ? *";

        var startDate = DateTime.UtcNow.AddDays(-2);
        var endDate = DateTime.UtcNow.AddDays(-1);

        //Act
        var result = TriggersFactory.CreateWithCron(triggerKey, cron, startDate, endDate);

        //Assert
        Assert.True(result is ITrigger);               
    }

    [Fact]
    public void CreateWithCron_ItShould_Return_Right_TriggerKey_Value_For_TriggerId_And_Group()
    {
        //Arrange 
        var triggerName = "triggerKey";
        var group = "groupName";
        var cron = "0 35 15 1/1 * ? *";

        var startDate = DateTime.UtcNow.AddDays(-2);
        var endDate = DateTime.UtcNow.AddDays(-1);

        //Act
        var result = TriggersFactory.CreateWithCron(triggerName, group, cron, startDate, endDate);

        //Assert
        Assert.True(result is ITrigger);       
    }
}

