using System.Text;

namespace ScheduleService.Infrastructure.QuartzManagement;
public class DateToCronConvertor
{
    private const string EVERY_TIME = "1/1";

    public static string GetDailyCron(DateTime date)
    {
        var textBuilder = new StringBuilder("0 ");
        textBuilder.Append($"{date.Minute} ");
        textBuilder.Append($"{date.Hour} ");
        textBuilder.Append($"{EVERY_TIME} ");
        textBuilder.Append($"* ");
        textBuilder.Append($"? ");
        textBuilder.Append($"*");

        return textBuilder.ToString();
    }

    public static string GetDailyCronByDaysOfWeek(DateTime date, List<DayOfWeek> weekDays)
    {
        if (weekDays.Count == 0)
            return string.Empty;

        var intDays = weekDays.Select(d => (int)d + 1).OrderBy(d => d);
        var weekDaysString = string.Join(",", intDays);

        var textBuilder = new StringBuilder("0 ");
        textBuilder.Append($"{date.Minute} ");
        textBuilder.Append($"{date.Hour} ");
        textBuilder.Append($"? ");
        textBuilder.Append($"* ");
        textBuilder.Append($"{weekDaysString} ");
        textBuilder.Append($"*");

        return textBuilder.ToString();
    }

    public static string GetWeeklyCron(DateTime date, DayOfWeek dayOfWeek)
    {
        var textBuilder = new StringBuilder("0 ");
        textBuilder.Append($"{date.Minute} ");
        textBuilder.Append($"{date.Hour} ");
        textBuilder.Append($"? ");
        textBuilder.Append($"* ");
        textBuilder.Append($"{(int)dayOfWeek + 1} ");
        textBuilder.Append($"*");

        return textBuilder.ToString();
    }

    public static string GetMonthlyCron(DateTime date)
    {
        var textBuilder = new StringBuilder("0 ");
        textBuilder.Append($"{date.Minute} ");
        textBuilder.Append($"{date.Hour} ");
        textBuilder.Append($"{date.Day} ");
        textBuilder.Append($"{EVERY_TIME} ");
        textBuilder.Append($"? ");
        textBuilder.Append($"*");

        return textBuilder.ToString();
    }
}
