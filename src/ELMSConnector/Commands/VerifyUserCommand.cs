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

namespace ElmsConnector.Commands
{
    using System;

    public class VerifyUserCommand : AbstractCommandBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IElmsSessionRequestService elmsSessionRequestService;

        public VerifyUserCommand(IAuthenticationService service, IElmsSessionRequestService elmsSessionRequestService)
        {
            authenticationService = service;
            this.elmsSessionRequestService = elmsSessionRequestService;
        }

        public override void Execute()
        {
            var username = Request["username"];
            var password = Request["password"];

            bool loginResult = authenticationService.AuthenticateUser(username, password);
            var token = (string) Session["token"];
            var returnUrl = (string) Session["returnUrl"];
            if (loginResult)
            {
                var login = new SuccessfulLogin(Response, elmsSessionRequestService);
                login.Execute(token, username, returnUrl);
            }
            else
            {
                string url = String.Format("Login.{2}?error=true&return_url={0}&token={1}", returnUrl, token, FileExtensionProvider.Extension);
                Response.Redirect(url);
            }
        }
    }
}