namespace ElmsConnector.Tests
{
    using System;
    using Castle.Core.Logging;
    using ELMSConnector;
    using Xunit;

    public class ElmsLoginService_Fixture
    {
        [Fact]
        public void CanAuthenticateUser()
        {
            string campus = "imgc_info";
            var service = new SessionTokenService(campus);
            service.Logger = new ConsoleLogger("Test_Run");

            var token = service.GetSessionToken();

            var loginService = new ElmsLoginService(campus);
            string url = loginService.GetRedirectUrl(token, "tigraine");
            Console.WriteLine("URL: {0}", url);
        }
    }
}