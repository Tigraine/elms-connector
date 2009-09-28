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

namespace ElmsConnector.Commands
{
    using System;
    using System.IO;
    using System.Net;
    using Castle.Core.Logging;

    public class VerifyUserCommand : ICommand
    {
        private readonly IAuthenticatonService _authenticatonService;
        private readonly IHttpRequest _request;
        private readonly IHttpResponse _response;
        private readonly IHttpSession _session;
        private readonly string _cgiConnector;
        private ILogger _logger = NullLogger.Instance;

        public VerifyUserCommand(IAuthenticatonService service, IHttpRequest request, IHttpResponse response,
                                 IHttpSession session, string cgiConnector)
        {
            _authenticatonService = service;
            _request = request;
            _response = response;
            _session = session;
            _cgiConnector = cgiConnector;
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public void Execute()
        {
            var username = _request["username"];
            var password = _request["password"];

            bool loginResult = _authenticatonService.AuthenticateUser(username, password);
            var token = _session["token"];
            var returnUrl = _session["returnUrl"];
            if (loginResult)
            {
                var url = String.Format("{0}&token={1}&uid={2}", returnUrl, token, username);

                string requestUri = String.Format("{0}&token={1}&uid={2}&groups={3}&department={4}", _cgiConnector,  
                                              token, username, "Student", "Department");
                WebRequest request = HttpWebRequest.Create(requestUri);

                _response.Write("URL for request: " + requestUri);
                WebResponse response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                string elmsResponse = reader.ReadToEnd();

                _response.Write(elmsResponse);


                _response.Write(String.Format("<a href=\"{0}\">{0}</a>", url));
                //_response.Redirect(url);
            }
            else
            {
                string url = String.Format("Login.elms?error=true&return_url={0}&token={1}", returnUrl, token);
                _response.Redirect(url);
            }
        }
    }
}