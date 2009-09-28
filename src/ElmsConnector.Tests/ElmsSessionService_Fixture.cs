namespace ElmsConnector.Tests
{
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using Services;
    using Xunit;

    public class ElmsSessionService_Fixture
    {
        [Fact]
        public void CallsElmsServerWithCorrectToken()
        {
            var requestServiceMock = MockRepository.GenerateMock<IRemoteRequestService>();
            var service = new ElmsSessionRequestService(requestServiceMock, "bla");
            requestServiceMock.Stub(p => p.RequestUri(null)).IgnoreArguments().Return("0 Account created");
            service.OpenSession("1234", "tig");

            requestServiceMock.AssertWasCalled(p => p.RequestUri(null), p => p.Constraints(Text.Contains("&token=1234")));
        }
    }
}