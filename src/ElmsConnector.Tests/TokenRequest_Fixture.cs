namespace ElmsConnector.Tests
{
    using Castle.Core.Logging;
    using ELMSConnector;
    using Xunit;

    public class TokenRequest_Fixture
    {
        [Fact]
        public void CanRequestToken()
        {
            var service = new SessionTokenService("imgc_info");
            service.Logger = new ConsoleLogger("Test_Run");
            string token = service.GetSessionToken();
            Assert.NotEmpty(token);
        }
    }
}
