namespace ElmsConnector.Tests
{
    using System;
    using Castle.Core.Logging;
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

            requestServiceMock.AssertWasCalled(p => p.RequestUri(null),
                                               p => p.Constraints(Text.Contains("&department=testDepartment")));
        }

        [Fact]
        public void CallsCorrectCgiConnectorUrl()
        {
            var requestServiceMock = MockRepository.GenerateMock<IRemoteRequestService>();
            requestServiceMock.Stub(p => p.RequestUri(null)).IgnoreArguments().Return("0 Account created");
            var service = new ElmsSessionRequestService(requestServiceMock, "specialCGIConnectorUrl");

            service.OpenSession("1234", "tig");

            requestServiceMock.AssertWasCalled(p => p.RequestUri(null),
                                               p => p.Constraints(Text.StartsWith("specialCGIConnectorUrl")));
        }

        [Fact]
        public void ReturnsTrue_IfServiceReturns_AccountUpdated()
        {
            var requestServiceMock = MockRepository.GenerateStub<IRemoteRequestService>();
            requestServiceMock.Stub(p => p.RequestUri(null)).IgnoreArguments().Return("0 Account updated");
            var service = new ElmsSessionRequestService(requestServiceMock, "bla");

            bool result = service.OpenSession("1234", "tig");

            Assert.Equal(true, result);
        }

        [Fact]
        public void ReturnsTrue_IfServiceReturns_AccountCreated()
        {
            var requestService = MockRepository.GenerateStub<IRemoteRequestService>();
            requestService.Stub(p => p.RequestUri(null)).IgnoreArguments().Return(SuccessResult);
            var service = new ElmsSessionRequestService(requestService, "bla");

            bool result = service.OpenSession("1234", "tig");

            Assert.Equal(true, result);
        }

        [Fact]
        public void OpenSession_RethrowsException()
        {
            var requestService = MockRepository.GenerateStub<IRemoteRequestService>();
            requestService.Stub(p => p.RequestUri(null)).IgnoreArguments().Throw(
                new ArgumentException("Test exception"));

            var service = new ElmsSessionRequestService(requestService, "bla");

            Assert.Throws<ArgumentException>(() => service.OpenSession("1234", "tig"));
        }

        [Fact]
        public void OpenSession_LogsExceptionToLogger()
        {
            var requestService = MockRepository.GenerateStub<IRemoteRequestService>();
            requestService.Stub(p => p.RequestUri(null)).IgnoreArguments().Throw(
                new ArgumentException("Test exception"));
            var logger = MockRepository.GenerateMock<ILogger>();
            var service = new ElmsSessionRequestService(requestService, "bla");
            service.Logger = logger;

            try
            {
                service.OpenSession("1234", "tig");
            }
            catch (Exception)
            {
            }

            logger.AssertWasCalled(p => p.ErrorFormat((Exception)null, null), p => p.IgnoreArguments());
        }
    }
}