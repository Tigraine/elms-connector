namespace ElmsConnector.Tests
{
    using System;
    using Rhino.Mocks.Interfaces;
    using Services;
    using Xunit;

    public class DateTimeProvider_Fixture
    {
        [Fact]
        public void NowReturnsUTC()
        {
            var provider = new DateTimeService();
            Assert.Equal(provider.Now, DateTime.UtcNow);
        }
    }
}