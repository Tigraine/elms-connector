namespace ElmsConnector.Tests
{
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
    }
}