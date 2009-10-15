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
        private LoginCommand CreateStubbedCommand()
        {
            var command = new LoginCommand();
            command.Request = MockRepository.GenerateStub<IHttpRequest>();
            command.Response = MockRepository.GenerateStub<IHttpResponse>();
            command.Session = MockRepository.GenerateStub<IHttpSession>();
            command.TemplateProvider = MockRepository.GenerateStub<ITemplateProvider>();
            command.FileExtensionProvider = MockRepository.GenerateStub<IFileExtensionProvider>();
            return command;
        }

        [Fact]
        public void LoginCommand_WritesContentOfTemplateToResponse()
        {
            var expected = "expected response";
            var response = MockRepository.GenerateMock<IHttpResponse>();    //Mock object
            var command = CreateStubbedCommand();
            command.Response = response;
            command.TemplateProvider.Stub(p => p.GetTemplate("Login")).Return(expected);

            command.Execute();

            response.AssertWasCalled(p => p.Write(expected));
        }

        [Fact]
        public void LoginCommand_WritesTokenToSession()
        {
            var expected = "121212";

            var command = CreateStubbedCommand();
            command.Request.Stub(p => p["token"]).Return(expected);
            command.Session = MockRepository.GenerateMock<IHttpSession>(); //Mock object

            command.Execute();

            command.Session.AssertWasCalled(p => p["token"] = expected);
        }

        [Fact]
        public void LoginCommand_WritesReturnUrlToSession()
        {
            var expected = "https://test.com/msdnaa.aspx?campus=abc";
            var command = CreateStubbedCommand();
            command.Request.Stub(p => p["return_url"]).Return(expected);
            command.Session = MockRepository.GenerateMock<IHttpSession>(); //Mock object

            command.Execute();

            command.Session.AssertWasCalled(p => p["returnUrl"] = expected);
        }

        [Fact]
        public void LoginCommand_WritesTokenToDebugLog()
        {
            var token = "121212";
            var command = CreateStubbedCommand();
            command.Request.Stub(p => p["token"]).Return(token);
            command.Logger = MockRepository.GenerateMock<ILogger>();

            command.Execute();

            command.Logger.AssertWasCalled(p => p.DebugFormat("Recieved Token {0}", token));
        }

        [Fact]
        public void LoginCommand_ReplacesErrorPlaceHolderInTemplate_WithNothingIfNoError()
        {
            var command = CreateStubbedCommand();
            command.TemplateProvider.Stub(p => p.GetTemplate(null)).IgnoreArguments().Return("<h1>$ERROR$</h1>");
            
            command.Execute();

            command.Response.AssertWasCalled(p => p.Write("<h1></h1>"));
        }

        [Fact]
        public void LoginCommand_PlacesErrorTextInPlaceHolder_WhenErrorIsTrue()
        {
            var command = CreateStubbedCommand();
            command.TemplateProvider.Stub(p => p.GetTemplate(null)).IgnoreArguments().Return("$ERROR$");
            command.Request.Stub(p => p["error"]).Return("true");
            command.Response = MockRepository.GenerateMock<IHttpResponse>();

            command.Execute();

            command.Response.AssertWasCalled(p => p.Write("<p class=\"error\">Login failed</p>"));
        }
    }
}