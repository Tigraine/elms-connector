namespace ElmsConnector.Tests
{
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using Services;
    using Xunit;

    public class ElmsSessionService_Fixture
    {
        private const string SuccessResult = "0 Account created";

        [Fact]
        public void CallsElmsServerWithCorrectToken()
        {
            var requestServiceMock = MockRepository.GenerateMock<IRemoteRequestService>();
            requestServiceMock.Stub(p => p.RequestUri(null)).IgnoreArguments().Return(SuccessResult);
            var service = new ElmsSessionRequestService(requestServiceMock, "bla");
            
            service.OpenSession("1234", "tig");

            requestServiceMock.AssertWasCalled(p => p.RequestUri(null), p => p.Constraints(Text.Contains("&token=1234")));
        }

        [Fact]
        public void CallsElmsServerWithCorrectUid()
        {
            var requestServiceMock = MockRepository.GenerateMock<IRemoteRequestService>();
            requestServiceMock.Stub(p => p.RequestUri(null)).IgnoreArguments().Return(SuccessResult);
            var service = new ElmsSessionRequestService(requestServiceMock, "bla");

            service.OpenSession("1234", "tig");

            requestServiceMock.AssertWasCalled(p => p.RequestUri(null), p => p.Constraints(Text.Contains("&uid=tig")));
        }

        [Fact]
        public void DepartmentCanBeOptinallySet()
        {
            var requestServiceMock = MockRepository.GenerateMock<IRemoteRequestService>();
            requestServiceMock.Stub(p => p.RequestUri(null)).IgnoreArguments().Return(SuccessResult);
            var service = new ElmsSessionRequestService(requestServiceMock, "bla");
            
            service.Department = "testDepartment";
            service.OpenSession("1234", "tig");

            requestServiceMock.AssertWasCalled(p => p.RequestUri(null), p => p.Constraints(Text.Contains("&department=testDepartment")));
        }

        /*
         * TODO: Add Tests for the following scenarios:
         * DepartmentCanBeSet
         * CallsCorrectCgiConnectorUrl
         * ReturnsTrue_IfServiceReturns_AccountCreated
         * ReturnsTrue_IfServiceReturns_AccountUpdated
         * LogsExceptionAndRethrows
         * */
    }
}