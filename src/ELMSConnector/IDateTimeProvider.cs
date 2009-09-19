namespace ELMSConnector
{
    using System;

    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}