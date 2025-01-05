using SharedKernel;

namespace ScheduleService.Domain.Shared;

public static class ErrorsSchedule
{
    public static class Schedule
    {
        public static Error RenewalIsDisabled()
        {
            return Error.Failure("renewal.is.disabled", "Renewal is disabled");
        }

        public static Error RenewalIsEnabled()
        {
            return Error.Failure("renewal.is.disabled", "Renewal is enabled");
        }

        public static Error InvalidPlannedEvent()
        {
            return Error.Failure(
                "invalid.planned.event",
                "The start date of the event exceeds the end date of the Schedule ");
        }
    }
}
