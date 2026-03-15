namespace Web.API.BackgroundServices
{
    public class RedmineSyncScheduleOptions
    {
        public bool Enabled { get; set; } = true;
        public int IntervalHours { get; set; } = 24;
        public int TimeEntryLookBackDays { get; set; } = 30;
        public int InitialDelaySeconds { get; set; } = 30;
    }
}
