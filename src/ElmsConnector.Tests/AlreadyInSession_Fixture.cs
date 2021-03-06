namespace ElmsConnector.Tests
{
    using Commands;
    using Rhino.Mocks;
    using Xunit;

    public class AlreadyInSession_Fixture
    {
        private LoginCommand CreateStubbedCommand(IExtendedAuthenticationService service, IElmsSessionRequestService elmsSessionRequestService)
        {
            var command = new LoginCommand(service, elmsSessionRequestService);
            command.Request = MockRepository.GenerateStub<IHttpRequest>();
            command.Response = MockRepository.GenerateStub<IHttpResponse>();
            command.Session = MockRepository.GenerateStub<IHttpSession>();
            command.TemplateProvider = MockRepository.GenerateStub<ITemplateProvider>();
            command.FileExtensionProvider = MockRepository.GenerateStub<IFileExtensionProvider>();
            return command;
        }

        [Fact]
        public void LoginCommandQueriesSessionServiceIfUserIsAlreadyAuthenticated()
        {
            var service = MockRepository.GenerateMock<IExtendedAuthenticationService>();
            var command = CreateStubbedCommand(service, MockRepository.GenerateStub<IElmsSessionRequestService>());

            command.Execute();

            service.AssertWasCalled(p => p.IsAlreadyAuthenticated());
        }

        [Fact(Skip = "?")]
        public void LoginCommandQueriesUsernameIfUserIsAlreadyAuthenticated()
        {
            var service = MockRepository.GenerateMock<IExtendedAuthenticationService>();
            service.Stub(p => p.IsAlreadyAuthenticated()).Return(true);
            var command = CreateStubbedCommand(service, MockRepository.GenerateStub<IElmsSessionRequestService>());

            command.Execute();
            
            service.AssertWasCalled(p => p.Username);
        }

        [Fact(Skip = "?")]
        public void LoginCommandExecutesSuccessfulLoginIfUserIsAlreadyAuthenticated()
        {
            var service = MockRepository.GenerateStub<IExtendedAuthenticationService>();
            service.Stub(p => p.IsAlreadyAuthenticated()).Return(true);
            var mock = MockRepository.GenerateMock<IElmsSessionRequestService>();
            var command = CreateStubbedCommand(service, mock);

            command.Execute();

            mock.AssertWasCalled(p => p.OpenSession(null, null), p => p.IgnoreArguments());
        }
    }
}