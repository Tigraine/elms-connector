/* Copyright 2009 Daniel H�lbling
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *     
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace ElmsConnector.Tests
{
    using Commands;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using Services;
    using Xunit;

    public class VerifyUserCommand_Fixture
    {
        private IAuthenticationService stubAuthService;
        private IElmsSessionRequestService stubSessionRequestService;
        public VerifyUserCommand CreateCommand(IAuthenticationService authService, IElmsSessionRequestService elmsSessionRequestService)
        {
            stubAuthService = authService;
            stubSessionRequestService = elmsSessionRequestService;

            var command = new VerifyUserCommand(authService, elmsSessionRequestService);
            command.Response = MockRepository.GenerateStub<IHttpResponse>();
            command.Request = MockRepository.GenerateStub<IHttpRequest>();
            command.Session = MockRepository.GenerateStub<IHttpSession>();
            command.FileExtensionProvider = MockRepository.GenerateStub<IFileExtensionProvider>();
            return command;
        }
        public VerifyUserCommand CreateCommand()
        {
            return CreateCommand(MockRepository.GenerateStub<IAuthenticationService>(),
                                 MockRepository.GenerateStub<IElmsSessionRequestService>());
        }

        public VerifyUserCommand CreateCommand(IAuthenticationService authService)
        {
            return CreateCommand(authService, MockRepository.GenerateStub<IElmsSessionRequestService>());
        }

        public VerifyUserCommand CreateCommand(IElmsSessionRequestService elmsSessionRequestService)
        {
            return CreateCommand(MockRepository.GenerateStub<IAuthenticationService>(), elmsSessionRequestService);
        }
            

        [Fact]
        public void VerifyUser_CallsAuthenticationServiceWithUsernameAndPassword()
        {
            const string username = "tigraine";
            const string password = "abc";


            var authService = MockRepository.GenerateMock<IAuthenticationService>();
            var command = CreateCommand(authService);
            command.Request.Stub(p => p["username"]).Return(username);
            command.Request.Stub(p => p["password"]).Return(password);
            
            command.Execute();

            authService.AssertWasCalled(p => p.AuthenticateUser(username, password));
        }

        [Fact]
        public void VerifyUser_ReadsUsernameAndPasswordFromRequest()
        {
            var command = CreateCommand();
            command.Request = MockRepository.GenerateMock<IHttpRequest>();
            
            command.Execute();

            //These two parameters are explicit and are required!
            command.Request.AssertWasCalled(p => p["username"]);
            command.Request.AssertWasCalled(p => p["password"]);
        }

        [Fact]
        public void VerifyUser_RedirectsToResponseBody_IfAuthServiceIsOk()
        {
            string token = "121212";
            var command = CreateCommand();
            var returnUrl = "https://test.com/ELMS.aspx";
            stubAuthService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(true);
            stubSessionRequestService.Stub(p => p.OpenSession(null, null)).IgnoreArguments().Return(
                new ElmsSessionServiceResponse() {HasSucceeded = true, ResponseBody = returnUrl});
            
            command.Execute();

            command.Response.AssertWasCalled(p => p.Redirect(returnUrl));
        }
        
        [Fact]
        public void VerifyUser_RedirectsToLoginWithErrorParam_WhenAuthenticationFalse()
        {
            var command = CreateCommand();
            stubAuthService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(false);
            command.Response = MockRepository.GenerateMock<IHttpResponse>();

            command.Execute();

            command.Response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.Contains("Login.?error=true")));
        }

        [Fact]
        public void VerifyUser_RedirectsWithReturnUrl_WhenAuthenticationFalse()
        {
            string token = "123123";
            var command = CreateCommand();
            stubAuthService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(false);
            command.Session["token"] = token;
            command.Response = MockRepository.GenerateMock<IHttpResponse>();
            
            command.Execute();

            command.Response.AssertWasCalled(p => p.Redirect(null), p => p.Constraints(Text.Contains("&token=" + token)));
        }

        [Fact]
        public void VerifyUser_RedirectsWithToken_WhenAuthenticationFalse()
        {
            string returnUrl = "http://test.com/elms.aspx";
            var command = CreateCommand();
            stubAuthService.Stub(p => p.AuthenticateUser(null, null)).IgnoreArguments().Return(false);
            command.Session["returnUrl"] = returnUrl;
            command.Response = MockRepository.GenerateMock<IHttpResponse>();
            
            command.Execute();

            command.Response.AssertWasCalled(p => p.Redirect(null),
                                     p => p.Constraints(Text.Contains("&return_url=" + returnUrl)));
        }
    }
}