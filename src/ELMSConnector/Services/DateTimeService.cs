namespace ElmsConnector.Services
{
    using System;

    public class DateTimeService : IDateTimeService
    {
        public DateTime Now
        {
            get { return DateTime.UtcNow; }
        }

        public static IDateTimeService Default
        {
            get { return new DateTimeService(); }
        }
    }
}