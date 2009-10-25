namespace ElmsConnector.Tests
{
    using Commands;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using Services;
    using Xunit;

    public class FileExtension_Fixture
    {
        [Fact]
        public void FileExtensionIsCorrectlySetInLoginView()
        {
            var command = new LoginCommand();
            var responseMock = MockRepository.GenerateMock<IHttpResponse>();
            command.Response = responseMock;
            var templateProvider = MockRepository.GenerateStub<ITemplateProvider>();
            templateProvider.Stub(p => p.GetTemplate(null)).IgnoreArguments().Return("$EXTENSION$");
            command.TemplateProvider = templateProvider;
            command.FileExtensionProvider = new FileExtensionProvider() { Extension = "EXT"};
            command.Request = MockRepository.GenerateStub<IHttpRequest>();
            command.Session = MockRepository.GenerateStub<IHttpSession>();

            command.Execute();

            responseMock.AssertWasCalled(p => p.Write("EXT"));
        }

        [Fact]
        public void VerifyUserRedirectsToCorrectFileExtension()
        {
            var authService = MockRepository.GenerateStub<IAuthenticationService>();
            var command = new VerifyUserCommand(authService, MockRepository.GenerateStub<IElmsSessionRequestService>());
            command.Response = MockRepository.GenerateMock<IHttpResponse>(); //Mock
            command.Request = MockRepository.GenerateStub<IHttpRequest>();
            command.Session = MockRepository.GenerateStub<IHttpSession>();
            command.FileExtensionProvider = MockRepository.GenerateStub<IFileExtensionProvider>();
            authService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(false);
            command.FileExtensionProvider.Stub(p => p.Extension).Return("EXT");
            
            command.Execute();

            command.Response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.StartsWith("Login.EXT")));
        }
    }
}