/* Copyright 2009 Daniel Hölbling
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
    using Castle.Core.Logging;
    using Commands;
    using Rhino.Mocks;
    using Xunit;

    public class LoginCommand_Fixture
    {
        [Fact]
        public void LoginCommand_WritesContentOfTemplateToResponse()
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
        public void LoginCommand_WritesTokenToSession()
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
        public void LoginCommand_WritesReturnUrlToSession()
        {
            var expected = "https://test.com/msdnaa.aspx?campus=abc";
            var session = MockRepository.GenerateMock<IHttpSession>(); //Mock object
            var request = MockRepository.GenerateStub<IHttpRequest>();
            request.Stub(p => p["return_url"]).Return(expected);

            var command = new LoginCommand(request,
                                           MockRepository.GenerateStub<IHttpResponse>(),
                                           session,
                                           MockRepository.GenerateStub<ITemplateProvider>());

            command.Execute();

            session.AssertWasCalled(p => p["returnUrl"] = expected);
        }

        [Fact]
        public void LoginCommand_WritesTokenToDebugLog()
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