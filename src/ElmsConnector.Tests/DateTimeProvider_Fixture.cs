namespace ElmsConnector.Tests
{
    using System;
    using ELMSConnector;
    using Xunit;

    public class DateTimeProvider_Fixture
    {
        [Fact]
        public void NowReturnsUTC()
        {
            var provider = new DateTimeProvider();
            Assert.Equal(provider.Now, DateTime.UtcNow);
        }
    }
}