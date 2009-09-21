namespace ElmsConnector.Tests
{
    using Castle.Core.Logging;
    using Commands;
    using Rhino.Mocks;
    using Xunit;

    public class LoginCommand_Fixture
    {
        [Fact]
        public void LoginCommandWritesContentOfTemplateToResponse()
        {
            var expected = "expected response";
            var templateProvider = MockRepository.GenerateStub<ITemplateProvider>();
            templateProvider.Stub(p => p.GetTemplate("Login")).Return(expected);
            var response = MockRepository.GenerateMock<IHttpResponse>();    //Mock object
            var command = new LoginCommand(MockRepository.GenerateStub<IHttpRequest>(), 
                                           response,
                                           MockRepository.GenerateStub<IHttpSession>(),
                                           templateProvider);

            command.Execute();

            response.AssertWasCalled(p => p.Write(expected));
        }

        [Fact]
        public void LoginCommandWritesTokenToSession()
        {
            var expected = "121212";
            var session = MockRepository.GenerateMock<IHttpSession>(); //Mock object
            var request = MockRepository.GenerateStub<IHttpRequest>();
            request.Stub(p => p["token"]).Return(expected);

            var command = new LoginCommand(request,
                                           MockRepository.GenerateStub<IHttpResponse>(),
                                           session,
                                           MockRepository.GenerateStub<ITemplateProvider>());

            command.Execute();

            session.AssertWasCalled(p => p["token"] = expected);
        }

        [Fact]
        public void LoginCommandWritesTokenToDebugLog()
        {
            var token = "121212";
            var request = MockRepository.GenerateStub<IHttpRequest>();
            request.Stub(p => p["token"]).Return(token);

            var command = new LoginCommand(request,
                                           MockRepository.GenerateStub<IHttpResponse>(),
                                           MockRepository.GenerateStub<IHttpSession>(),
                                           MockRepository.GenerateStub<ITemplateProvider>());
            var mockLogger = MockRepository.GenerateMock<ILogger>();
            command.Logger = mockLogger;

            command.Execute();

            mockLogger.AssertWasCalled(p => p.DebugFormat("Recieved Token {0}", token));
        }

        [Fact]
        public void LoginCommand_ReplacesErrorPlaceHolderInTemplate_WithNothingIfNoError()
        {
            var response = MockRepository.GenerateMock<IHttpResponse>();
            var templateProvider = MockRepository.GenerateStub<ITemplateProvider>();
            templateProvider.Stub(p => p.GetTemplate(null)).IgnoreArguments().Return("<h1>$ERROR$</h1>");
            var command = new LoginCommand(MockRepository.GenerateStub<IHttpRequest>(),
                                           response,
                                           MockRepository.GenerateStub<IHttpSession>(),
                                           templateProvider);
            
            command.Execute();

            response.AssertWasCalled(p => p.Write("<h1></h1>"));
        }

        [Fact]
        public void LoginCommand_PlacesErrorTextInPlaceHolder_WhenErrorIsTrue()
        {
            var response = MockRepository.GenerateMock<IHttpResponse>();
            var templateProvider = MockRepository.GenerateStub<ITemplateProvider>();
            templateProvider.Stub(p => p.GetTemplate(null)).IgnoreArguments().Return("$ERROR$");
            var request = MockRepository.GenerateStub<IHttpRequest>();
            request.Stub(p => p["error"]).Return("true");
            var command = new LoginCommand(request,
                                           response,
                                           MockRepository.GenerateStub<IHttpSession>(),
                                           templateProvider);

            command.Execute();

            response.AssertWasCalled(p => p.Write("<p class=\"error\">Login failed</p>"));
        }
    }
}