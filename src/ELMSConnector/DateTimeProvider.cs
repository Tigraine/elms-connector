namespace ELMSConnector
{
    using System;

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.UtcNow; }
        }

        public static IDateTimeProvider Default
        {
            get { return new DateTimeProvider(); }
        }
    }
}