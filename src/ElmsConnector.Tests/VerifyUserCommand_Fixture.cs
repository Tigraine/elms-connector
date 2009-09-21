namespace ElmsConnector.Tests
{
    using Commands;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using Xunit;

    public class VerifyUserCommand_Fixture
    {
        [Fact]
        public void VerifyUserCallsAuthenticationServiceWithUsernameAndPassword()
        {
            const string username = "tigraine";
            const string password = "abc";

            var authService = MockRepository.GenerateMock<IAuthenticatonService>();
            var httpRequest = MockRepository.GenerateStub<IHttpRequest>();
            var response = MockRepository.GenerateStub<IHttpResponse>();
            var sessionService = MockRepository.GenerateStub<IHttpSession>();
            httpRequest.Stub(p => p["username"]).Return(username);
            httpRequest.Stub(p => p["password"]).Return(password);
            var command = new VerifyUserCommand(authService, httpRequest, response, sessionService);

            command.Execute();

            authService.AssertWasCalled(p => p.AuthenticateUser(username, password));
        }

        [Fact]
        public void VerifyUserReadsUsernameAndPasswordFromRequest()
        {
            var authService = MockRepository.GenerateStub<IAuthenticatonService>();
            var httpRequest = MockRepository.GenerateMock<IHttpRequest>();
            var sessionService = MockRepository.GenerateStub<IHttpSession>();
            var response = MockRepository.GenerateStub<IHttpResponse>();
            var command = new VerifyUserCommand(authService, httpRequest, response, sessionService);

            command.Execute();

            //These two parameters are explicit and are required!
            httpRequest.AssertWasCalled(p => p["username"]);
            httpRequest.AssertWasCalled(p => p["password"]);
        }

        [Fact]
        public void VerifyUserRedirectsToReturnUrlWithTokenIfAuthServiceIsOk()
        {
            string token = "121212";
            var authService = MockRepository.GenerateStub<IAuthenticatonService>();
            authService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(true);
            var httpRequest = MockRepository.GenerateStub<IHttpRequest>();
            var sessionService = MockRepository.GenerateStub<IHttpSession>();
            var response = MockRepository.GenerateMock<IHttpResponse>();
            sessionService["token"] = token;
            sessionService["returnUrl"] = "https://test.com/ELMS.aspx";
            var command = new VerifyUserCommand(authService, httpRequest, response, sessionService);

            command.Execute();

            response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.Contains("&token=" + token)));
        }

        [Fact]
        public void VerifyUser_RedirectsToReturnUrlWithUidIsUsername_IfAuthServiceIsOk()
        {
            var username = "tigraine";
            var authService = MockRepository.GenerateStub<IAuthenticatonService>();
            authService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(true);
            var httpRequest = MockRepository.GenerateStub<IHttpRequest>();
            httpRequest.Stub(p => p["username"]).Return(username);
            var sessionService = MockRepository.GenerateStub<IHttpSession>();
            var response = MockRepository.GenerateMock<IHttpResponse>();
            sessionService["returnUrl"] = "https://test.com/ELMS.aspx";
            var command = new VerifyUserCommand(authService, httpRequest, response, sessionService);

            command.Execute();

            response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.Contains("&uid=" + username)));
        }

        [Fact]
        public void VerifyUser_RedirectsToLoginWithErrorParam_WhenAuthenticationFalse()
        {
            var authService = MockRepository.GenerateStub<IAuthenticatonService>();
            authService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(false);
            var httpRequest = MockRepository.GenerateStub<IHttpRequest>();
            var sessionService = MockRepository.GenerateStub<IHttpSession>();
            var response = MockRepository.GenerateMock<IHttpResponse>();
            var command = new VerifyUserCommand(authService, httpRequest, response, sessionService);

            command.Execute();

            response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.Contains("Login.elms?error=true")));
        }

        [Fact]
        public void VerifyUser_RedirectsWithReturnUrl_WhenAuthenticationFalse()
        {
            string token = "123123";
            var authService = MockRepository.GenerateStub<IAuthenticatonService>();
            authService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(false);
            var httpRequest = MockRepository.GenerateStub<IHttpRequest>();
            var sessionService = MockRepository.GenerateStub<IHttpSession>();
            sessionService["token"] = token;
            var response = MockRepository.GenerateMock<IHttpResponse>();
            var command = new VerifyUserCommand(authService, httpRequest, response, sessionService);

            command.Execute();

            response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.Contains("&token=" + token)));
        }
        
        [Fact]
        public void VerifyUser_RedirectsWithToken_WhenAuthenticationFalse()
        {
            string returnUrl = "http://test.com/elms.aspx";
            var authService = MockRepository.GenerateStub<IAuthenticatonService>();
            authService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(false);
            var httpRequest = MockRepository.GenerateStub<IHttpRequest>();
            var sessionService = MockRepository.GenerateStub<IHttpSession>();
            sessionService["returnUrl"] = returnUrl;
            var response = MockRepository.GenerateMock<IHttpResponse>();
            var command = new VerifyUserCommand(authService, httpRequest, response, sessionService);

            command.Execute();

            response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.Contains("&return_url=" + returnUrl)));
        }
    }
}