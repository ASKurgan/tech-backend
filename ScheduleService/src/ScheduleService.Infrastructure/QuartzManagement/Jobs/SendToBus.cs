using ScheduleService.Infrastructure.QuartzManagement.Abstratction;

namespace ScheduleService.Infrastructure.QuartzManagement.Jobs;

public class SendToBus : ISendToBus
{
    void ISendToBus.SendToBus(string data)
    {
        Console.WriteLine($"Send to bus: {data}");
    }
}
