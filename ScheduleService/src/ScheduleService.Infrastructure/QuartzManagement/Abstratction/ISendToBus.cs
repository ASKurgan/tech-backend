namespace ScheduleService.Infrastructure.QuartzManagement.Abstratction;

//interface for scheduleed task
public interface ISendToBus
{
    void SendToBus(string data);
}
