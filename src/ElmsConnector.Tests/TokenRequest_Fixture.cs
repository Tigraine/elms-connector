namespace ElmsConnector.Tests
{
    using System;
    using Castle.Core.Logging;
    using ELMSConnector;
    using Rhino.Mocks;
    using Xunit;

    public class TokenRequest_Fixture
    {
        [Fact]
        public void CanRequestToken()
        {
            var service = new SessionTokenService("imgc_info");
            service.Logger = new ConsoleLogger("Test_Run");

            var token = service.GetSessionToken();

            Assert.NotNull(token);
        }

        [Fact]
        public void TokenExpiresIn5Minutes()
        {
            var service = new SessionTokenService("imgc_info");
            service.Logger = new ConsoleLogger("Test_Run");
            service.DateTimeService = MockRepository.GenerateStub<IDateTimeProvider>();
            var tokenRequestDate = new DateTime(2000, 1, 1);
            service.DateTimeService.Expect(p => p.Now).Return(tokenRequestDate).Repeat.Any();


            var token = service.GetSessionToken();

            Assert.Equal(token.Expiration, tokenRequestDate.AddMinutes(5));
        }
    }
}